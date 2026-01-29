"""
Unit tests for ConversationManager
"""
import pytest
from unittest.mock import AsyncMock, MagicMock, patch
from agent.conversation import ConversationManager
from agent.config import AgentConfig
from agent.backend_client import BackendClient


@pytest.fixture
def mock_backend():
    """Create a mock BackendClient"""
    backend = MagicMock(spec=BackendClient)
    backend.search_faqs = AsyncMock()
    backend.log_unanswered_question = AsyncMock()
    return backend


@pytest.fixture
def conversation_manager(mock_backend):
    """Create a ConversationManager instance for testing"""
    config = AgentConfig(
        agent_name="Meridian Concierge",
        backend_api_url="http://localhost:5000",
        openai_api_key="test-key",
        openai_chat_model="gpt-4",
        confidence_threshold=0.7
    )
    return ConversationManager(config=config, backend=mock_backend)


@pytest.mark.asyncio
async def test_get_greeting(conversation_manager):
    """Test greeting generation"""
    greeting = await conversation_manager.get_greeting()
    
    assert isinstance(greeting, str)
    assert len(greeting) > 0
    assert "Meridian" in greeting or "concierge" in greeting.lower()


@pytest.mark.asyncio
async def test_process_question_with_high_confidence(conversation_manager, mock_backend):
    """Test processing question with high confidence match"""
    # Mock FAQ search result
    mock_backend.search_faqs.return_value = [
        {
            "question": "What are your check-in hours?",
            "answer": "Check-in is from 3 PM to 11 PM.",
            "category": "Hotel Services",
            "similarity": 0.95
        }
    ]
    
    # Mock OpenAI completion
    with patch('agent.conversation.OpenAI') as mock_openai:
        mock_client = MagicMock()
        mock_completion = MagicMock()
        mock_completion.choices = [
            MagicMock(message=MagicMock(content="Check-in is from 3 PM to 11 PM."))
        ]
        mock_client.chat.completions.create = AsyncMock(return_value=mock_completion)
        mock_openai.return_value = mock_client
        
        response = await conversation_manager.process_question("When is check-in?")
        
        assert isinstance(response, str)
        assert len(response) > 0
        mock_backend.search_faqs.assert_called_once()
        mock_backend.log_unanswered_question.assert_not_called()


@pytest.mark.asyncio
async def test_process_question_with_low_confidence(conversation_manager, mock_backend):
    """Test processing question with low confidence match"""
    # Mock FAQ search result with low confidence
    mock_backend.search_faqs.return_value = [
        {
            "question": "Some unrelated question",
            "answer": "Some answer",
            "category": "Other",
            "similarity": 0.3
        }
    ]
    mock_backend.log_unanswered_question.return_value = True
    
    # Mock OpenAI completion
    with patch('agent.conversation.OpenAI') as mock_openai:
        mock_client = MagicMock()
        mock_completion = MagicMock()
        mock_completion.choices = [
            MagicMock(message=MagicMock(content="I'll check with a staff member."))
        ]
        mock_client.chat.completions.create = AsyncMock(return_value=mock_completion)
        mock_openai.return_value = mock_client
        
        response = await conversation_manager.process_question("What's the weather?")
        
        assert isinstance(response, str)
        mock_backend.log_unanswered_question.assert_called_once()


@pytest.mark.asyncio
async def test_process_question_no_results(conversation_manager, mock_backend):
    """Test processing question with no FAQ results"""
    # Mock empty FAQ search result
    mock_backend.search_faqs.return_value = []
    mock_backend.log_unanswered_question.return_value = True
    
    # Mock OpenAI completion
    with patch('agent.conversation.OpenAI') as mock_openai:
        mock_client = MagicMock()
        mock_completion = MagicMock()
        mock_completion.choices = [
            MagicMock(message=MagicMock(content="Let me connect you with staff."))
        ]
        mock_client.chat.completions.create = AsyncMock(return_value=mock_completion)
        mock_openai.return_value = mock_client
        
        response = await conversation_manager.process_question("Random question?")
        
        assert isinstance(response, str)
        mock_backend.log_unanswered_question.assert_called_once()


@pytest.mark.asyncio
async def test_build_context_from_faqs(conversation_manager):
    """Test building context from FAQ results"""
    faqs = [
        {
            "question": "Q1",
            "answer": "A1",
            "category": "Cat1",
            "similarity": 0.9
        },
        {
            "question": "Q2",
            "answer": "A2",
            "category": "Cat2",
            "similarity": 0.8
        }
    ]
    
    context = conversation_manager._build_context_from_faqs(faqs)
    
    assert isinstance(context, str)
    assert "Q1" in context
    assert "A1" in context
    assert "Q2" in context
    assert "A2" in context

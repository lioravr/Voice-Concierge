"""
Unit tests for BackendClient
"""
import pytest
from unittest.mock import AsyncMock, MagicMock, patch
import httpx
from agent.backend_client import BackendClient


@pytest.fixture
def backend_client():
    """Create a BackendClient instance for testing"""
    return BackendClient(base_url="http://localhost:5000", timeout=10.0)


@pytest.mark.asyncio
async def test_health_check_success(backend_client):
    """Test successful health check"""
    with patch('httpx.AsyncClient.get') as mock_get:
        mock_response = MagicMock()
        mock_response.status_code = 200
        mock_response.json.return_value = {"status": "Healthy"}
        mock_get.return_value = mock_response
        
        result = await backend_client.health_check()
        
        assert result is True
        mock_get.assert_called_once()


@pytest.mark.asyncio
async def test_health_check_failure(backend_client):
    """Test failed health check"""
    with patch('httpx.AsyncClient.get') as mock_get:
        mock_get.side_effect = httpx.RequestError("Connection failed")
        
        result = await backend_client.health_check()
        
        assert result is False


@pytest.mark.asyncio
async def test_search_faqs_success(backend_client):
    """Test successful FAQ search"""
    with patch('httpx.AsyncClient.get') as mock_get:
        mock_response = MagicMock()
        mock_response.status_code = 200
        mock_response.json.return_value = [
            {
                "id": 1,
                "question": "What are check-in hours?",
                "answer": "Check-in is from 3 PM to 11 PM.",
                "category": "Hotel Services"
            }
        ]
        mock_get.return_value = mock_response
        
        result = await backend_client.search_faqs("check-in time", limit=5)
        
        assert len(result) == 1
        assert result[0]["question"] == "What are check-in hours?"


@pytest.mark.asyncio
async def test_search_faqs_empty_result(backend_client):
    """Test FAQ search with no results"""
    with patch('httpx.AsyncClient.get') as mock_get:
        mock_response = MagicMock()
        mock_response.status_code = 200
        mock_response.json.return_value = []
        mock_get.return_value = mock_response
        
        result = await backend_client.search_faqs("unknown question")
        
        assert result == []


@pytest.mark.asyncio
async def test_get_active_voice_success(backend_client):
    """Test retrieving active voice configuration"""
    with patch('httpx.AsyncClient.get') as mock_get:
        mock_response = MagicMock()
        mock_response.status_code = 200
        mock_response.json.return_value = {
            "id": 1,
            "name": "James",
            "providerVoiceId": "alloy",
            "speed": 1.0,
            "isActive": True
        }
        mock_get.return_value = mock_response
        
        result = await backend_client.get_active_voice()
        
        assert result is not None
        assert result["name"] == "James"
        assert result["providerVoiceId"] == "alloy"


@pytest.mark.asyncio
async def test_get_active_voice_not_found(backend_client):
    """Test when no active voice is configured"""
    with patch('httpx.AsyncClient.get') as mock_get:
        mock_response = MagicMock()
        mock_response.status_code = 404
        mock_get.return_value = mock_response
        
        result = await backend_client.get_active_voice()
        
        assert result is None


@pytest.mark.asyncio
async def test_log_unanswered_question_success(backend_client):
    """Test logging unanswered question"""
    with patch('httpx.AsyncClient.post') as mock_post:
        mock_response = MagicMock()
        mock_response.status_code = 201
        mock_response.json.return_value = {"id": 1, "question": "Test question"}
        mock_post.return_value = mock_response
        
        result = await backend_client.log_unanswered_question(
            question="Test question",
            session_id="session-123"
        )
        
        assert result is True
        mock_post.assert_called_once()


@pytest.mark.asyncio
async def test_close(backend_client):
    """Test closing the client"""
    with patch.object(backend_client._client, 'aclose', new_callable=AsyncMock) as mock_close:
        await backend_client.close()
        mock_close.assert_called_once()

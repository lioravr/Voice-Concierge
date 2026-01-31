"""
Custom LLM implementation that queries PostgreSQL FAQ database
Integrates with LiveKit Agents SDK v1.3.x
"""
import structlog
import asyncio
from typing import Optional
from livekit.agents import llm

from .conversation import ConversationManager

logger = structlog.get_logger()


class FAQLLM(llm.LLM):
    """
    Custom LLM that searches PostgreSQL FAQ database before generating responses.
    
    This replaces the standard OpenAI LLM with one that:
    1. Searches the FAQ database using semantic search
    2. Records unanswered questions when similarity < 0.7
    3. Generates natural responses using the FAQ context
    """
    
    def __init__(self, conversation_manager: ConversationManager):
        super().__init__()
        self.conversation = conversation_manager
        logger.info("faq_llm_initialized_with_postgresql_backend")
    
    async def chat(
        self,
        chat_ctx: llm.ChatContext,
        conn_options: Optional[dict] = None,
        **kwargs
    ) -> llm.LLMStream:
        """
        Process chat request by querying FAQ database.
        
        Args:
            chat_ctx: Chat context with conversation history
            conn_options: Optional connection options
            **kwargs: Additional parameters (ignored)
        
        Returns:
            LLMStream with FAQ-based response
        """
        # Extract the last user message
        user_messages = [msg for msg in chat_ctx.messages if msg.role == "user"]
        
        if not user_messages:
            logger.warning("no_user_message_in_context")
            # Return empty response
            return _EmptyStream(self, chat_ctx)
        
        last_question = user_messages[-1].content
        logger.info("processing_question_with_faq_database", question=last_question[:50])
        
        try:
            # Use ConversationManager to:
            # 1. Search PostgreSQL FAQ database with semantic search
            # 2. Record unanswered questions if similarity < 0.7
            # 3. Generate natural response using GPT-4
            response_text = await self.conversation.process_question(last_question)
            
            logger.info("faq_response_generated", 
                       question_length=len(last_question),
                       response_length=len(response_text))
            
            # Return stream with FAQ response
            return _FAQResponseStream(self, chat_ctx, response_text)
        
        except Exception as e:
            logger.error("faq_processing_error", error=str(e), question=last_question[:50])
            
            # Return fallback response
            fallback = "I apologize, but I'm having trouble accessing information right now. Please contact the front desk for assistance."
            return _FAQResponseStream(self, chat_ctx, fallback)


class _FAQResponseStream(llm.LLMStream):
    """Stream that yields a single FAQ response"""
    
    def __init__(self, llm_instance: llm.LLM, chat_ctx: llm.ChatContext, response_text: str):
        super().__init__(llm_instance, chat_ctx, None)
        self._response_text = response_text
        self._yielded = False
    
    async def __anext__(self) -> llm.ChatChunk:
        """Yield the FAQ response as a single chunk"""
        if self._yielded:
            raise StopAsyncIteration
        
        self._yielded = True
        
        # Return a complete response chunk
        return llm.ChatChunk(
            request_id="faq_response",
            choices=[
                llm.Choice(
                    delta=llm.ChoiceDelta(
                        role="assistant",
                        content=self._response_text
                    ),
                    index=0
                )
            ]
        )
    
    async def aclose(self):
        """Close the stream"""
        pass


class _EmptyStream(llm.LLMStream):
    """Empty stream for when there's no user message"""
    
    def __init__(self, llm_instance: llm.LLM, chat_ctx: llm.ChatContext):
        super().__init__(llm_instance, chat_ctx, None)
    
    async def __anext__(self) -> llm.ChatChunk:
        """Immediately stop iteration"""
        raise StopAsyncIteration
    
    async def aclose(self):
        """Close the stream"""
        pass

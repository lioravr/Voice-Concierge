"""
Conversation management and LLM integration
"""
import structlog
from typing import List, Dict, Any, Optional
from openai import AsyncOpenAI

from .backend_client import BackendClient
from .config import AgentConfig

logger = structlog.get_logger()


class ConversationManager:
    """Manages conversation flow and LLM interactions"""
    
    def __init__(self, config: AgentConfig, backend: BackendClient):
        self.config = config
        self.backend = backend
        self.openai = AsyncOpenAI(api_key=config.openai_api_key)
        
        # System prompt for the voice concierge
        self.system_prompt = """You are a professional voice concierge for The Meridian Casino & Resort in Las Vegas.

Your role:
- Provide warm, friendly, and professional service
- Answer guest questions accurately using the knowledge base provided
- Be conversational and natural, not robotic
- Keep responses concise (2-3 sentences max for voice)
- If you don't know something, be honest and offer to record the question

Personality:
- Professional but approachable
- Enthusiastic about the resort
- Attentive to guest needs
- Conversational and natural

Important guidelines:
- ALWAYS use the FAQ knowledge base provided in the context
- Keep responses brief for voice interaction
- Use natural, conversational language
- Avoid technical jargon
- If information isn't in the FAQs, be honest and offer to record the question

Remember: You're speaking, not writing. Keep it natural and conversational!"""
        
        # Conversation history
        self.messages: List[Dict[str, str]] = []
    
    async def process_question(self, question: str) -> str:
        """
        Process a user question and generate a response
        
        Args:
            question: User's question
        
        Returns:
            Response text to speak back
        """
        logger.info("processing_question", question=question)
        
        # Search for relevant FAQs
        faq_results = await self.backend.search_faqs(
            query=question,
            limit=self.config.search_limit,
            threshold=self.config.similarity_threshold
        )
        
        # Build context from FAQs
        faq_context = self._build_faq_context(faq_results)
        
        # Determine if we found relevant information
        has_relevant_info = len(faq_results) > 0 and faq_results[0].get("similarity", 0) > 0.7
        
        if not has_relevant_info:
            # Record unanswered question
            await self.backend.record_unanswered_question(question)
            logger.info("question_recorded_as_unanswered", question=question)
        
        # Generate response using LLM
        response = await self._generate_llm_response(
            question=question,
            faq_context=faq_context,
            has_relevant_info=has_relevant_info
        )
        
        # Add to conversation history
        self.messages.append({"role": "user", "content": question})
        self.messages.append({"role": "assistant", "content": response})
        
        # Keep conversation history manageable (last 10 exchanges)
        if len(self.messages) > 20:
            self.messages = self.messages[-20:]
        
        logger.info("question_processed", question=question, response_length=len(response))
        
        return response
    
    def _build_faq_context(self, faq_results: List[Dict[str, Any]]) -> str:
        """Build context string from FAQ search results"""
        if not faq_results:
            return "No relevant information found in knowledge base."
        
        context_parts = ["Here is relevant information from the knowledge base:\n"]
        
        for idx, result in enumerate(faq_results, 1):
            faq = result.get("faq", {})
            similarity = result.get("similarity", 0)
            
            question = faq.get("question", "")
            answer = faq.get("answer", "")
            category = faq.get("category", "General")
            
            context_parts.append(
                f"{idx}. [{category}] Q: {question}\n"
                f"   A: {answer}\n"
                f"   (Similarity: {similarity:.2%})\n"
            )
        
        return "\n".join(context_parts)
    
    async def _generate_llm_response(
        self,
        question: str,
        faq_context: str,
        has_relevant_info: bool
    ) -> str:
        """Generate response using OpenAI LLM"""
        
        # Build user message with context
        if has_relevant_info:
            user_message = f"""Guest question: {question}

{faq_context}

Please provide a natural, conversational response based on the information above. Keep it brief (2-3 sentences) since this is voice interaction."""
        else:
            user_message = f"""Guest question: {question}

{faq_context}

The knowledge base doesn't have specific information about this. Please politely let the guest know you'll record their question for the team, and ask if there's anything else you can help with."""
        
        try:
            # Prepare messages for LLM
            messages = [
                {"role": "system", "content": self.system_prompt},
                *self.messages[-10:],  # Include recent context
                {"role": "user", "content": user_message}
            ]
            
            # Call OpenAI
            response = await self.openai.chat.completions.create(
                model="gpt-4",  # Can use gpt-3.5-turbo for cost savings
                messages=messages,
                max_tokens=150,  # Keep responses concise for voice
                temperature=0.7,  # Balanced creativity
            )
            
            assistant_message = response.choices[0].message.content or ""
            
            logger.info(
                "llm_response_generated",
                prompt_tokens=response.usage.prompt_tokens if response.usage else 0,
                completion_tokens=response.usage.completion_tokens if response.usage else 0
            )
            
            return assistant_message.strip()
        
        except Exception as e:
            logger.error("llm_generation_error", error=str(e))
            
            # Fallback response
            if has_relevant_info:
                # Try to use first FAQ answer directly
                first_faq = faq_results[0].get("faq", {}) if faq_results else {}
                answer = first_faq.get("answer", "")
                if answer:
                    return answer
            
            return self.config.fallback_message
    
    def reset_conversation(self):
        """Reset conversation history"""
        self.messages = []
        logger.info("conversation_reset")
    
    async def get_greeting(self) -> str:
        """Get personalized greeting message"""
        # Could fetch voice configuration and customize greeting
        voice_config = await self.backend.get_active_voice()
        
        if voice_config:
            voice_name = voice_config.get("name", "your concierge")
            greeting = f"Hello! I'm {voice_name}, your voice concierge for The Meridian Casino & Resort. How may I assist you today?"
            return greeting
        
        return self.config.greeting_message

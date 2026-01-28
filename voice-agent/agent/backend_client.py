"""
Backend API client for FAQ search and unanswered question recording
"""
import httpx
import structlog
from typing import List, Optional, Dict, Any

logger = structlog.get_logger()


class BackendClient:
    """Client for communicating with the Voice Concierge backend API"""
    
    def __init__(self, base_url: str, timeout: int = 10):
        self.base_url = base_url.rstrip("/")
        self.timeout = timeout
        self.client = httpx.AsyncClient(
            base_url=self.base_url,
            timeout=timeout,
            headers={"Content-Type": "application/json"}
        )
    
    async def search_faqs(
        self,
        query: str,
        limit: int = 3,
        threshold: float = 0.3
    ) -> List[Dict[str, Any]]:
        """
        Search FAQs using semantic search
        
        Args:
            query: User's question
            limit: Maximum number of results
            threshold: Similarity threshold (0-1)
        
        Returns:
            List of FAQ search results with similarity scores
        """
        try:
            response = await self.client.post(
                "/api/faq/search",
                json={
                    "query": query,
                    "limit": limit,
                    "threshold": threshold
                }
            )
            response.raise_for_status()
            results = response.json()
            
            logger.info(
                "faq_search_success",
                query=query,
                result_count=len(results)
            )
            
            return results
        
        except httpx.HTTPStatusError as e:
            logger.error(
                "faq_search_http_error",
                query=query,
                status_code=e.response.status_code,
                error=str(e)
            )
            return []
        
        except Exception as e:
            logger.error(
                "faq_search_error",
                query=query,
                error=str(e)
            )
            return []
    
    async def record_unanswered_question(self, question: str) -> bool:
        """
        Record an unanswered question for later review
        
        Args:
            question: The question that couldn't be answered
        
        Returns:
            True if recorded successfully, False otherwise
        """
        try:
            response = await self.client.post(
                "/api/unansweredquestions",
                json={"question": question}
            )
            response.raise_for_status()
            
            logger.info(
                "unanswered_question_recorded",
                question=question
            )
            
            return True
        
        except Exception as e:
            logger.error(
                "record_unanswered_error",
                question=question,
                error=str(e)
            )
            return False
    
    async def get_active_voice(self) -> Optional[Dict[str, Any]]:
        """
        Get the currently active voice configuration
        
        Returns:
            Voice configuration dict or None
        """
        try:
            response = await self.client.get("/api/voiceconfigurations/active")
            response.raise_for_status()
            voice = response.json()
            
            logger.info(
                "active_voice_retrieved",
                voice_name=voice.get("name"),
                voice_id=voice.get("voiceId")
            )
            
            return voice
        
        except httpx.HTTPStatusError as e:
            if e.response.status_code == 404:
                logger.warning("no_active_voice_configured")
            else:
                logger.error(
                    "get_active_voice_error",
                    status_code=e.response.status_code,
                    error=str(e)
                )
            return None
        
        except Exception as e:
            logger.error(
                "get_active_voice_error",
                error=str(e)
            )
            return None
    
    async def health_check(self) -> bool:
        """
        Check if backend API is healthy
        
        Returns:
            True if healthy, False otherwise
        """
        try:
            response = await self.client.get("/health")
            return response.status_code == 200
        except Exception as e:
            logger.error("backend_health_check_failed", error=str(e))
            return False
    
    async def close(self):
        """Close the HTTP client"""
        await self.client.aclose()

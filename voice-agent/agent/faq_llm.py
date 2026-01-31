"""
Custom LLM that answers from the PostgreSQL FAQ database (via backend API).

This integrates with LiveKit Agents SDK v1.3.x by implementing `llm.LLM.chat()`
and returning a proper `llm.LLMStream` (which must implement `_run()`).
"""

from __future__ import annotations

from typing import Any

import structlog
from livekit.agents import llm
from livekit.agents.types import DEFAULT_API_CONNECT_OPTIONS, NOT_GIVEN, APIConnectOptions, NotGivenOr

from .conversation import ConversationManager

logger = structlog.get_logger()


class FAQLLM(llm.LLM):
    """
    LLM adapter that delegates answering to `ConversationManager.process_question()`.
    """

    def __init__(self, conversation_manager: ConversationManager):
        super().__init__()
        self._conversation = conversation_manager
        logger.info("faq_llm_initialized_with_postgresql_backend")

    def chat(
        self,
        *,
        chat_ctx: llm.ChatContext,
        tools: list[llm.Tool] | None = None,
        conn_options: APIConnectOptions = DEFAULT_API_CONNECT_OPTIONS,
        parallel_tool_calls: NotGivenOr[bool] = NOT_GIVEN,
        tool_choice: NotGivenOr[llm.ToolChoice] = NOT_GIVEN,
        extra_kwargs: NotGivenOr[dict[str, Any]] = NOT_GIVEN,
    ) -> llm.LLMStream:
        # We currently ignore tool calling options, but we accept them to keep
        # compatibility with the LiveKit Agents SDK `LLM.chat()` signature.
        _ = parallel_tool_calls, tool_choice, extra_kwargs

        return _FAQResponseStream(
            self,
            chat_ctx=chat_ctx,
            tools=tools or [],
            conn_options=conn_options,
            conversation=self._conversation,
        )


class _FAQResponseStream(llm.LLMStream):
    """
    A one-shot stream that emits exactly one assistant message.

    LiveKit Agents expects `LLM.chat()` to return an `LLMStream` instance that
    implements `_run()` and emits `ChatChunk`s via `self._event_ch`.
    """

    def __init__(
        self,
        llm_instance: llm.LLM,
        *,
        chat_ctx: llm.ChatContext,
        tools: list[llm.Tool],
        conn_options: APIConnectOptions,
        conversation: ConversationManager,
    ) -> None:
        # Extract the last user message BEFORE starting the stream task (super().__init__)
        # In livekit-agents v1.3.x, ChatContext uses 'items' (not 'messages')
        # and content may be a list, so we use text_content property
        user_messages = [msg for msg in chat_ctx.items if msg.role == "user"]
        self._question = ""
        if user_messages:
            last_msg = user_messages[-1]
            # text_content joins all text parts; fallback to content if needed
            if hasattr(last_msg, "text_content"):
                self._question = last_msg.text_content or ""
            elif isinstance(last_msg.content, list):
                self._question = " ".join(str(c) for c in last_msg.content)
            else:
                self._question = str(last_msg.content) if last_msg.content else ""
        self._conversation = conversation

        super().__init__(
            llm_instance,
            chat_ctx=chat_ctx,
            tools=tools,
            conn_options=conn_options,
        )

    async def _run(self) -> None:
        if not self._question:
            logger.warning("no_user_message_in_context")
            return

        logger.info("processing_question_with_faq_database", question=self._question[:80])

        try:
            response_text = await self._conversation.process_question(self._question)
        except Exception as e:
            logger.error("faq_processing_error", error=str(e), question=self._question[:80])
            response_text = (
                "I apologize, but I'm having trouble accessing information right now. "
                "Please contact the front desk for assistance."
            )

        self._event_ch.send_nowait(
            llm.ChatChunk(
                id="faq_response",
                delta=llm.ChoiceDelta(role="assistant", content=response_text),
            )
        )

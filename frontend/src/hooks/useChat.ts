import { useState, useCallback, useRef } from 'react';
import { v4 as uuidv4 } from 'uuid';
import type { Message, ChatState } from '../types/chat';
import { streamChat } from '../services/chatApi';

const SESSION_ID = uuidv4(); // one session per browser tab

export function useChat() {
  const [state, setState] = useState<ChatState>({
    sessionId: SESSION_ID,
    messages: [],
    isStreaming: false,
    error: null,
  });

  const abortControllerRef = useRef<AbortController | null>(null);

  const sendMessage = useCallback(async (userText: string) => {
    if (!userText.trim() || state.isStreaming) return;

    const userMessage: Message = {
      id: uuidv4(),
      role: 'user',
      content: userText.trim(),
      timestamp: new Date(),
    };

    const assistantId = uuidv4();
    const assistantMessage: Message = {
      id: assistantId,
      role: 'assistant',
      content: '',
      timestamp: new Date(),
    };

    setState(prev => ({
      ...prev,
      messages: [...prev.messages, userMessage, assistantMessage],
      isStreaming: true,
      error: null,
    }));

    abortControllerRef.current = new AbortController();

    try {
      for await (const token of streamChat(
        SESSION_ID,
        userText,
        abortControllerRef.current.signal
      )) {
        setState(prev => ({
          ...prev,
          messages: prev.messages.map(m =>
            m.id === assistantId ? { ...m, content: m.content + token } : m
          ),
        }));
      }
    } catch (err) {
      if ((err as Error).name === 'AbortError') return;
      setState(prev => ({
        ...prev,
        error: (err as Error).message,
        messages: prev.messages.map(m =>
          m.id === assistantId
            ? { ...m, content: '[Error: Failed to get response]' }
            : m
        ),
      }));
    } finally {
      setState(prev => ({ ...prev, isStreaming: false }));
      abortControllerRef.current = null;
    }
  }, [state.isStreaming]);

  const stopStreaming = useCallback(() => {
    abortControllerRef.current?.abort();
  }, []);

  const clearMessages = useCallback(() => {
    setState(prev => ({ ...prev, messages: [], error: null }));
  }, []);

  return { state, sendMessage, stopStreaming, clearMessages };
}

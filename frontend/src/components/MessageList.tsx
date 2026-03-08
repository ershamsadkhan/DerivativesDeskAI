import { useEffect, useRef } from 'react';
import type { Message } from '../types/chat';
import { MessageBubble } from './MessageBubble';
import './MessageList.css';

interface Props {
  messages: Message[];
  isStreaming: boolean;
}

export function MessageList({ messages, isStreaming }: Props) {
  const bottomRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  if (messages.length === 0) {
    return (
      <div className="message-list empty-state">
        <div className="empty-icon">📊</div>
        <h3>Derivatives Desk AI</h3>
        <p>Ask me about futures contracts, rollover periods, or order activity.</p>
        <div className="example-prompts">
          <span>What contracts expire in March 2026?</span>
          <span>Which contracts are rolling over this week?</span>
          <span>Show cancelled orders for ESH26</span>
          <span>What orders did Alice Morgan place in February?</span>
        </div>
      </div>
    );
  }

  return (
    <div className="message-list">
      {messages.map((msg, idx) => (
        <MessageBubble
          key={msg.id}
          message={msg}
          isStreaming={isStreaming && idx === messages.length - 1 && msg.role === 'assistant'}
        />
      ))}
      <div ref={bottomRef} />
    </div>
  );
}

import type { Message } from '../types/chat';
import './MessageBubble.css';

interface Props {
  message: Message;
  isStreaming?: boolean;
}

export function MessageBubble({ message, isStreaming }: Props) {
  const isUser = message.role === 'user';

  return (
    <div className={`bubble-wrapper ${isUser ? 'user' : 'assistant'}`}>
      <div className="bubble-label">{isUser ? 'You' : 'Desk AI'}</div>
      <div className={`bubble ${isUser ? 'bubble-user' : 'bubble-assistant'}`}>
        <pre className="bubble-text">{message.content}</pre>
        {isStreaming && !isUser && (
          <span className="streaming-cursor" aria-label="streaming">▋</span>
        )}
      </div>
      <div className="bubble-time">
        {message.timestamp.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
      </div>
    </div>
  );
}

import { useChat } from '../hooks/useChat';
import { MessageList } from './MessageList';
import { InputBar } from './InputBar';
import './ChatBox.css';

export function ChatBox() {
  const { state, sendMessage, stopStreaming, clearMessages } = useChat();

  return (
    <div className="chatbox">
      <div className="chatbox-header">
        <div className="header-title">
          <span className="header-icon">📈</span>
          <span>Derivatives Desk AI</span>
        </div>
        <div className="header-actions">
          {state.isStreaming && (
            <span className="streaming-indicator">● Generating...</span>
          )}
          <button className="btn-clear" onClick={clearMessages} title="Clear conversation">
            Clear
          </button>
        </div>
      </div>

      {state.error && (
        <div className="error-banner">⚠ {state.error}</div>
      )}

      <MessageList messages={state.messages} isStreaming={state.isStreaming} />

      <InputBar
        onSend={sendMessage}
        onStop={stopStreaming}
        isStreaming={state.isStreaming}
      />
    </div>
  );
}

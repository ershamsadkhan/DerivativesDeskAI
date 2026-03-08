import { useState, KeyboardEvent, useRef } from 'react';
import './InputBar.css';

interface Props {
  onSend: (message: string) => void;
  onStop: () => void;
  isStreaming: boolean;
}

export function InputBar({ onSend, onStop, isStreaming }: Props) {
  const [text, setText] = useState('');
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  const handleSend = () => {
    if (!text.trim() || isStreaming) return;
    onSend(text.trim());
    setText('');
    if (textareaRef.current) {
      textareaRef.current.style.height = 'auto';
    }
  };

  const handleKeyDown = (e: KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  const handleInput = () => {
    const el = textareaRef.current;
    if (!el) return;
    el.style.height = 'auto';
    el.style.height = `${Math.min(el.scrollHeight, 160)}px`;
  };

  return (
    <div className="input-bar">
      <textarea
        ref={textareaRef}
        className="input-textarea"
        value={text}
        onChange={e => setText(e.target.value)}
        onKeyDown={handleKeyDown}
        onInput={handleInput}
        placeholder="Ask about futures contracts, rollovers, orders... (Enter to send, Shift+Enter for newline)"
        rows={1}
        disabled={false}
      />
      {isStreaming ? (
        <button className="btn-stop" onClick={onStop} title="Stop generation">
          ⬛ Stop
        </button>
      ) : (
        <button
          className="btn-send"
          onClick={handleSend}
          disabled={!text.trim()}
          title="Send (Enter)"
        >
          Send ↵
        </button>
      )}
    </div>
  );
}

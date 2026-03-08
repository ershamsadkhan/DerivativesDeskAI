export interface Message {
  id: string;
  role: 'user' | 'assistant';
  content: string;
  timestamp: Date;
}

export interface ChatState {
  sessionId: string;
  messages: Message[];
  isStreaming: boolean;
  error: string | null;
}

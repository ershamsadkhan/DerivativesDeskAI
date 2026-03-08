// Requests are proxied to the backend via vite.config.ts — no cross-origin issues
const API_BASE = '';

export async function* streamChat(
  sessionId: string,
  message: string,
  signal?: AbortSignal
): AsyncGenerator<string> {
  const response = await fetch(`${API_BASE}/api/chat/stream`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ sessionId, message }),
    signal,
  });

  if (!response.ok) {
    throw new Error(`API error: ${response.status} ${response.statusText}`);
  }

  const reader = response.body?.getReader();
  if (!reader) throw new Error('No response body');

  const decoder = new TextDecoder();
  let buffer = '';

  try {
    while (true) {
      const { done, value } = await reader.read();
      if (done) break;

      buffer += decoder.decode(value, { stream: true });
      const lines = buffer.split('\n');
      buffer = lines.pop() ?? '';

      for (const line of lines) {
        if (!line.startsWith('data: ')) continue;
        const data = line.slice(6).trim();
        if (data === '[DONE]') return;
        try {
          const parsed = JSON.parse(data);
          if (parsed.token) yield parsed.token as string;
          if (parsed.error) throw new Error(`Server error: ${parsed.error}`);
        } catch (e) {
          if (e instanceof Error && e.message.startsWith('Server error:')) throw e;
          // ignore other malformed lines
        }
      }
    }
  } finally {
    reader.releaseLock();
  }
}

export async function fetchHistory(sessionId: string) {
  const response = await fetch(`${API_BASE}/api/chat/history/${sessionId}`);
  if (!response.ok) throw new Error('Failed to fetch history');
  return response.json();
}

export async function triggerEtl() {
  const response = await fetch(`${API_BASE}/api/etl/trigger`, { method: 'POST' });
  return response.json();
}

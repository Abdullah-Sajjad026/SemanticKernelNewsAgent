# News AI Agent

A conversational AI agent that fetches and displays news from New York Times RSS feeds.

## Features

- Fetches real-time news from NY Times RSS feeds
- Maintains conversation context using session IDs
- Returns formatted news in consistent chat-style responses
- Supports multiple news categories (Technology, Business, Science, etc.)
- Clean, responsive web interface

## Demo
![live-demo](https://github.com/user-attachments/assets/2e1ab699-004f-457a-bab2-c386dc731424)


## Prerequisites

- .NET 7.0 or later
- A valid API key for Gemini

## Setup Instructions

### 1. Backend Setup

1. Clone this repository
   ```bash
   git clone https://github.com/Abdullah-Sajjad026/SemanticKernelNewsAgent.git
   cd news-ai-agent
   ```

2. Update the API configuration:
    - Open `appsettings.json`
    - Replace `"your-api-key"` with your actual API key:
      ```json
      "LLM": {
        "ApiKey": "your-actual-api-key-here",
        "Model": "gemini-2.5-pro"
      }
      ```

3. Run the backend API:
   ```bash
   dotnet run
   ```
   The API will start on `https://localhost:7131` by default.

### 2. Frontend Setup

1. Open `WebDemo.html` in your code editor

2. If you changed the backend port, update the API URL:
   ```javascript
   const ChatApiURL = "https://localhost:YOUR_PORT/api/Chat";
   ```

3. Open `WebDemo.html` in a web browser:
    - Simply double-click the file, or
    - Use a local server like Live Server in VS Code

## Usage

1. Type your request in the chat box (e.g. "technology news")
2. Press Enter or click Send
3. View formatted news results with:
    - Category tags
    - Publication dates
    - Headlines and summaries
    - Links to full articles

## Configuration Options

### Backend

- Modify `appsettings.json` to:
    - Change the LLM model
    - Adjust logging levels
    - Configure allowed hosts

### Frontend

- Edit `WebDemo.html` to:
    - Change the API endpoint URL
    - Modify UI colors and styles
    - Adjust session timeout duration

## Troubleshooting

- **CORS Errors**: Ensure your frontend URL is allowed in the backend CORS configuration
- **Empty Responses**: Verify your API key is valid and has proper permissions
- **Connection Issues**: Check that both frontend and backend are using the same protocol (HTTP/HTTPS)

## License

[MIT License](LICENSE)

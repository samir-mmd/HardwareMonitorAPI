# Hardware Monitor API
![image](https://github.com/user-attachments/assets/5b8b2b8c-7ef0-487d-84fb-1e619be4ba38)


## Overview
**Hardware Monitor API** is a .NET Core Web API designed to run on your computer at startup as a background process. It provides real-time hardware information accessible from any device with a web browser within the same local network. This API is perfect for repurposing old devices, like mobile phones or tablets, into dedicated hardware monitoring screens.

## Features
- **Automatic Startup**: Runs automatically on computer startup, ensuring that your hardware monitoring is always available.
- **Real-Time Data**: Provides live updates of various hardware metrics, including CPU load, GPU load, temperatures, fan speeds, and more.
- **Web-Based Interface**: No need for specialized software; access the monitoring dashboard from any device with a web browser.
- **SignalR Integration**: Uses SignalR for real-time communication, ensuring immediate updates without needing to refresh the page.
- **Local Network Access**: Securely access your hardware information from any device within your local network.
  
## Use Cases
- **Dedicated Monitoring Device**: Turn any device, such as an old mobile phone or tablet, into a dedicated hardware monitoring device.
- ![image](https://github.com/user-attachments/assets/9049e760-df5d-4b8e-984a-5fdf9e897721)

- **Remote Monitoring**: Keep an eye on your computer's hardware performance from anywhere in your home or office.

## Getting Started

### Prerequisites
- **.NET Core SDK**: Make sure you have the .NET Core SDK installed on your machine.

### Installation
1. **Clone the repository**:  
2.  **Build the project**
3.  **Run**
4. **Running on Startup
To ensure the API starts automatically with your computer, you can configure it to run on startup using your operating system's task scheduler.**

Accessing the Dashboard
Once the API is running, you can access the monitoring dashboard by opening a web browser on any device within the same local network and navigating to:
http://**your-pc-ip**:3005

Customization
Feel free to customize the HTML and CSS files to match your preferences. The UI is responsive and can be tailored to fit different screen sizes.

Contributing
Contributions are welcome! If you have any suggestions or improvements, please feel free to submit a pull request or open an issue.

Acknowledgments
Special thanks to all the contributors and the open-source community for their support and resources.

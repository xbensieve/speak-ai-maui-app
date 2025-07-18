# Speak-AI

**Speak-AI** is a cross-platform mobile application built with .NET MAUI that provides an interactive and engaging English learning experience. Leveraging real-time communication, Firebase integration, and modern UI design, Speak-AI delivers a robust and user-friendly platform for both Android and iOS.

---

## Technical Stack

### Frameworks and Platforms
- **.NET 8**: Modern and high-performance development platform.
- **.NET MAUI**: Unified project structure for Android and iOS.

### Libraries and Packages
- `CommunityToolkit.Maui`: Essential UI components.
- `CommunityToolkit.Mvvm`: Simplifies MVVM architecture.
- `Microsoft.AspNetCore.SignalR.Client`: Real-time communication.
- `Microsoft.Extensions.Http`: HTTP client and dependency injection support.
- `Newtonsoft.Json`: JSON serialization/deserialization.
- `UraniumUI.Material`: Material Design components.
- `Plugin.Firebase`: Firebase services integration.
- `Plugin.Firebase.Crashlytics`: Crash reporting support.

---

## Project Structure

### 1. `SpeakAI` (.NET MAUI Project)
Contains UI and platform-specific configuration.

**Key Folders:**
- `Resources/`
  - `AppIcon/`: Application icons.
  - `Splash/`: Splash screen assets (`splash.svg`).
  - `Images/`: Application images.
  - `Fonts/`: Custom fonts.
  - `Raw/`: Additional raw assets.
- `Views/`: XAML-based pages (e.g., `LoginPage.xaml`, `SignUpPage.xaml`, `CoursePage.xaml`) following MVVM pattern.

**Dependencies:**
- Firebase for analytics and crash reporting.
- SignalR for real-time communication.

---

### 2. `SpeakAI.Services`
Contains business logic and shared models.

**Key Components:**
- `CourseDetailModel`: Details about the course, topics, and exercises.
- `Topic`: Represents each course topic.
- `Exercise`: Stores exercise content and structure.

Built with clean MVVM architecture for maintainability and scalability.

---

## Key Features

- **Cross-Platform Support**: One codebase for Android and iOS via .NET MAUI.
- **Firebase Integration**: Analytics and crash reporting via Firebase.
- **Real-Time Communication**: SignalR enables real-time updates.
- **Material Design UI**: Built with UraniumUI for consistency and elegance.
- **Custom Resources**: Easily customize splash screen, icons, images, and fonts.

---

## How to Build and Run

### Prerequisites
- Visual Studio 2022 with **.NET MAUI** workload installed.
- Android and iOS emulators properly configured.

### Steps
1. **Clone the Repository**
   ```bash
   git clone https://github.com/xbensieve/english-ai-maui-app.git
   cd english-ai-maui-app
2. **Restore Dependencies**:
   Open the solution in Visual Studio and restore NuGet packages.
3. **Run the Application**:
   Select the target platform (Android or iOS) and click the "Run" button in Visual Studio.

---

## Future Enhancements

- **Cloud Sync**: Implement support for syncing user progress across devices, ensuring a seamless experience for users on different platforms.
  
- **Push Notifications**: Integrate **Firebase Cloud Messaging** (FCM) to deliver real-time notifications to users, enhancing engagement and communication.

- **Localization**: Expand the application's accessibility by adding support for multiple languages, aiming to reach a broader global audience.


---

Contributing
Contributions are welcome! Please follow these steps:
1.	Fork the repository.
2.	Create a feature branch.
3.	Submit a pull request with a detailed description of your changes.

---

License
This project is licensed under the MIT License. See the LICENSE file for details.
---
Contact
For questions or support, please contact [bennguyen.contact@gmail.com].

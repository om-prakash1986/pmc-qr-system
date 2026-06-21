# Mob App Work flow.docx

PMC PVC QR Card Activation App - Flutter Implementation Plan
We will create a Flutter mobile application for Patna Nagar Nigam (PMC) municipal staff to activate PVC QR plates on-site in the field.
The app will feature:
Secure Employee Authentication: Secure login for field agents.
Dashboard: Statistics, recent activations, sync status, and visual action buttons.
QR Scanner & Simulator: Real camera-based scanning with a built-in interactive simulator for testing on platforms without camera support (e.g. desktop/web/simulators).
PID Verification Form: Input and verify Property IDs (PIDs), pulling owner/address information to ensure accurate matching.
Security Logic (One-time Activation): Check status of QR codes (UNASSIGNED vs ACTIVATED), prevent reassignment, and handle supervisor bypass workflow.
Offline Queue Sync: Save scans locally if internet is unavailable, allowing syncing later.
User Review Required
IMPORTANT
Mock Database State: Since there is no backend API codebase in the workspace, the Flutter application will be built with a decoupled MockApiService and a local database (using shared_preferences for persistence) pre-populated with:
Valid PIDs (e.g. PTN-W15-H1001, PTN-W15-H1002, etc.) and corresponding owner/address data.
A list of QR Codes generated in Phase 1 (with matching tokens like 2D0F1150-CF04-45DF-A4B3-AF12F41209D7 from the database example).
Mock employee credentials (e.g. User: EMP101, Password: password).
This service layer will expose asynchronous API-like methods, making it extremely easy to replace with real HTTP requests (http or dio) when the backend is ready.
TIP
Cross-Platform Compatibility: To ensure the project builds and runs smoothly on all platforms (Android, iOS, Windows, and Web), camera-based packages can sometimes have platform limitations. We will implement:
A simulator mode that enables manual typing of QR URLs/Tokens or choosing from a dropdown list of pre-generated mock PVC cards.
An optional real scanning view using mobile_scanner that is wrapped to prevent app crashes on unsupported platforms.
Proposed Architecture & Directory Structure
We will create a new Flutter application named pmc_activation_app inside the workspace directory.
pmc_activation_app/
├── pubspec.yaml               # Package dependencies (provider, google_fonts, shared_preferences)
└── lib/
    ├── main.dart              # App entry point, material app, routing, providers
    ├── theme.dart             # Curated PMC aesthetic (deep blue, rich orange, emerald green, card overlays)
    ├── models/
    │   ├── qr_card.dart       # Model representing PVC QR Card details
    │   └── property.dart      # Model representing Property details
    ├── services/
    │   ├── mock_database.dart # Seed data for testing (PIDs, employees, QRs)
    │   ├── api_service.dart   # Async API-like service (simulates network delays & database operations)
    │   └── auth_service.dart  # Manages active employee session
    ├── screens/
    │   ├── login_screen.dart       # Premium glassmorphism login screen
    │   ├── home_screen.dart        # Dashboard, stats, offline status, and scan launch
    │   ├── scan_screen.dart        # Camera scanner interface + Interactive mock selector
    │   ├── activation_screen.dart  # Verification form (scanned QR data + PID entry + confirmation)
    │   └── success_screen.dart     # Confetti & visual celebration of successful link
    └── widgets/
        ├── custom_button.dart      # Reusable button with hover/tap animations
        └── status_badge.dart       # Visual status tag (Unassigned / Activated)
Core Flow & Verification Rules
AuthenticateTap ScanScan QR / Choose MockLookup QR DetailsACTIVATEDUNASSIGNEDClick VerifyNoYesConfirm & SaveStart AppLogin ScreenDashboardScan ScreenExtract QRToken/URLQR Status?Show Error: Already ActivatedEnter PIDPID Exists?Show Error: Invalid PIDShow Property & Owner DetailsUpdate Status to ACTIVATEDCelebration & Success Screen
Scan Validation:
The app parses the scanned string. It handles both full URLs (e.g. https://pmc.bihar.gov.in/qr/2D0F1150-CF04-45DF-A4B3-AF12F41209D7) and individual QRTokens/IDs.
It matches the token against the QRMaster database.
One-Time Activation Security:
If the QR card status is ACTIVATED, the UI shows a locked screen indicating the Property ID it is already linked to, the employee who linked it, and the date.
If the status is UNASSIGNED, the user is allowed to proceed.
PID Validation:
The user enters a PID. The app checks if this PID exists in the database.
If valid, it fetches details (Owner, Address, Ward). The staff verifies this visually.
Once confirmed, it updates the database, setting the state to ACTIVATED and recording ActivatedBy = EMP101 and ActivatedDate = DateTime.now().
Verification Plan
Automated & Manual Testing
We will verify the implementation by compiling and running the project:
Create and Setup: Create the Flutter project and configure pubspec.yaml with required dependencies.
Build Test: Run flutter build windows or flutter build web or flutter run to confirm compile-time correctness.
Interactive Testing (Simulator Mode):
Open the app.
Log in using credentials EMP101 and password.
On the Home screen, review statistics (e.g., active cards, pending syncs).
Go to "Scan" -> Use the Interactive Simulator Mode to select an unassigned QR card (e.g. PMC/IND/DQR/00000001 with token 2D0F1150-CF04-45DF-A4B3-AF12F41209D7).
Enter an invalid PID to test verification failure.
Enter a valid PID (e.g. PTN-W15-H1001) -> Verify that property owner info is loaded.
Click "Confirm & Activate" -> Verify the success screen is shown and statistics update.
Try to scan the same QR card again -> Verify the security logic stops the user, showing it's already assigned.
                                            Core Flow & Verification Rules
Authenticate
Tap Scan
Scan QR / Choose Mock
Lookup QR Details
ACTIVATED
UNASSIGNED
Click Verify
No
Yes
Confirm & Save
Start App
Login Screen
Dashboard
Scan Screen
Extract QRToken/URL
QR Status?
Show Error: Already Activated
Enter PID
PID Exists?
Show Error: Invalid PID
Show Property & Owner Details
Update Status to ACTIVATED
Celebration & Success Screen

---

# api work flow.docx

Walkthrough: PMC PVC QR Card Activation System
We have successfully created the C# backend Generic Handlers and the Flutter mobile application to support the Patna Nagar Nigam (PMC) smart PVC QR card field installation and activation workflow.
📁 System Components
1. ASP.NET C# Backend API Handlers
Located in the workspace at: backend_api/ These generic handlers (.ashx) connect to the pmc_sparrow_data database using the existing "PTax" connection string from Web.config:
LoginHandler.ashx: Authenticates field staff using SHA-256 password hashing.
CardLookupHandler.ashx: Looks up a scanned card by token (UUID/GUID) or QRId. Auto-extracts tokens from full QRUrls.
PropertyLookupHandler.ashx: Fetches holding records (Owner Name, Address, Ward, Dues) from the All_Demand table by Property ID (PID).
ActivateCardHandler.ashx: Runs an atomic transaction that validates unassigned card status, maps it to a verified PID, deactivates older mappings, stamps All_Demand, and logs the activation to tbl_QR_ScanLog.
2. Flutter Mobile Application (pmc_activation_app)
Located in the workspace at: pmc_activation_app/ Built as a responsive, premium cross-platform application utilizing the state provider pattern.
lib/main.dart: Core entrypoint, router, and provider injection.
lib/theme.dart: Unified HSL custom theme matching PMC aesthetics (deep blue, rich orange, emerald green).
lib/services/api_service.dart: Handles asynchronous API operations with a toggleable offline simulator database pre-loaded with testing accounts and sample holding rows.
lib/screens/:
login_screen.dart: Forms verification and session load.
home_screen.dart: Staff profile widgets, real-time statistics, sync queue, and settings panel.
scan_screen.dart: Views laser viewport scanning overlays and provides an interactive card simulator list.
activation_screen.dart: Implements PID check and holding cards with outstanding dues alert alerts.
success_screen.dart: Renders completion summaries and triggers return logs.
🛠️ Execution & Deployment Guide
C# Backend Integration
To integrate these handlers into your IIS web server, simply copy the files inside backend_api/ to the api/ directory of your C# ASP.NET project (e.g. c:\Users\hp\OneDrive\Desktop\QRPVCCard\api\). Ensure your project references System.Web and has connection strings matching "PTax".
Running the Flutter Mobile App
Make sure Flutter is ready (flutter doctor).
Run cd pmc_activation_app
Launch the app in development mode on your connected mobile device, emulator, desktop, or web browser:
powershell
flutter run -d chrome  # or windows / android
🧪 Verification Flow (Step-by-Step)
Phase A: Simulated Testing (No setup required!)
The application is pre-configured with Simulated Mode = Active so you can test the entire flow right away:
Staff Login: Use EMP101 and password password (or admin / Admin@1234).
Dashboard: Verify the stats showing Circle (Patliputra Circle) and Ward (1).
Scan QR: Tap Activate New Card to open the scanner view.
Choose Test Card: Tap Test Card 1 (Unassigned).
Verify PID:
Type a wrong PID (e.g. 9999) and tap Verify -> Verify it fails with a "PID not found" warning.
Tap Quick Mock PID: 1409113 -> Verify that property owner info is fetched (SRI KUMUD VISHAL, Shanti Vihar Colony, etc.).
Complete Activation: Tap Confirm & Activate Link -> Verify success screen details.
One-Time Lock Check:
Go back to Dashboard. Observe stats updated.
Go to Scan QR -> Tap Activated Card (Security Block Test) -> Verify that the security lock prevents activation, showing who activated it and its linked PID.
Phase B: Live API Server Integration
Copy handlers to your IIS root api/ folder.
In the Flutter App, swipe open the left menu drawer (Settings).
Switch Simulation Mode to Off.
Enter your deployed server URL (e.g. https://pmc.bihar.gov.in/api) and click Save Endpoint URL.
Log in and repeat the activation test -> Verify that operations update the tables in your remote database (Server=ws241.win.arvixe.com;Database=pmc_sparrow_data).

---

# sdk work flow.docx

Here is a detailed guide on how to build, transfer, and install the Flutter mobile app onto your physical Android phone (or distribute it to the PMC staff).
Step 1: Build the Android App Package (APK)
To generate the installation file (.apk) for Android, run the compilation command inside the Flutter project directory:
Open a terminal (PowerShell or Command Prompt).
Navigate to the Flutter directory:
powershell
cd "c:\Users\hp\OneDrive\Desktop\pmc pvc card work flow\pmc_activation_app"
Run the compiler command to generate a release build:
powershell
flutter build apk --release
This command compiles the Dart code into optimized machine code for mobile phones.
Once completed, your installer file will be created at: 📂 pmc_activation_app\build\app\outputs\flutter-apk\app-release.apk
Step 2: How to Install the App on a Mobile Phone
You have three main methods to get the compiled app-release.apk onto your phone and install it:
Method A: Direct Download via your IIS Server (Recommended for Staff)
Since your web server (ws241.win.arvixe.com) is already configured to host and serve .apk files (we saw the .apk mime-type mapping in your Web.config on line 247!), you can host it directly:
Copy the compiled app-release.apk from your computer.
Upload it to the root folder of your website on the hosting server.
Rename it to something clean, like pmc_activation.apk.
PMC Staff can download and install the app directly on their phones by visiting: http://pmc.bihar.gov.in/pmc_activation.apk (or your staging domain) in their mobile browser.
Method B: Direct USB Transfer (For testing on your own phone)
Connect your Android phone to your computer using a USB cable.
Change the USB connection mode on your phone to "File Transfer" (MTP).
Copy app-release.apk from pmc_activation_app\build\app\outputs\flutter-apk\ and paste it into any folder on your phone (e.g., the Downloads folder).
On your phone, open the Files or File Manager app, locate the .apk file, and tap it to install.
Method C: Send via WhatsApp Web / Email / Google Drive
Upload the .apk file to Google Drive, email it to yourself, or send it to your phone via WhatsApp Web.
Open the file on your phone to trigger the installation.
Step 3: Enabling Android Security Permission (First time only)
Since this app is distributed directly (sideloaded) and not through the Google Play Store, Android will show a security block on installation:
When you tap the .apk file, Android might say: "For security, your phone is not allowed to install unknown apps from this source."
Tap Settings in that popup.
Toggle the switch to "Allow from this source" (or enable "Install Unknown Apps" for your browser/file manager).
Go back and tap Install to finish.
Alternative: Live Testing with USB Debugging (Developer Mode)
If you want to run the app directly from your computer onto your USB-connected phone and see errors live:
Go to your phone's Settings -> About Phone -> Tap Build Number 7 times to enable Developer Options.
Go to Settings -> Developer Options -> Enable USB Debugging.
Connect the phone, open your terminal, and run:

---

# wORK FLOW OF PVC CARD.docx

Your Workflow
Phase 1: Mass Print PVC QR Plates
First, print thousands of PVC plates with a unique QR ID only.
Example:
PATNA NAGAR NIGAMQR ID: PMC00000001QR ID: PMC00000002QR ID: PMC00000003[ QR CODE ]
At this stage, the QR is not linked to any property.
Database:
QRId
Status
PropertyId
PMC00000001
Unassigned
NULL
PMC00000002
Unassigned
NULL
PMC00000003
Unassigned
NULL
Phase 2: Field Installation
A PMC employee visits a house.
He picks any unused PVC plate.
Example:
PMC00004567
Fixes it on the wall.
Scans the QR using the PMC mobile app.
Phase 3: Activation with PID
The app asks:
Enter PID (Property ID)
Example:
PID = PTN-W15-H1001
System verifies the PID exists.
Then updates:
QRId
Status
PropertyId
PMC00004567
Activated
PTN-W15-H1001
Also store:
QRId
PID
ActivatedBy
ActivationDate
PMC00004567
PTN-W15-H1001
EMP101
2026-06-17
After Activation
When someone scans:
PMC00004567      ↓Find PID      ↓Find Property Record      ↓Display Latest Property Data
Recommended Database
CREATE TABLE QRMaster(    QRId VARCHAR(20) PRIMARY KEY,    Status VARCHAR(20),    PropertyId VARCHAR(50),    ActivatedBy VARCHAR(50),    ActivatedDate DATETIME);
QR Content
The QR can simply contain:
https://pmc.gov.in/qr/PMC00004567
or
PMC00004567
The URL option is better because phones can open it directly.
Additional Security
Add a one-time activation rule:
Unassigned → Activated
After activation:
Activated → Cannot be reassigned
unless a supervisor approves.
This prevents staff from moving a QR from one property to another.
Best Practice for PMC
Generate 5–10 lakh unique QR IDs. 
Print PVC plates in bulk. 
Keep all QRs in "Unassigned" status. 
Fix any unused plate on any house. 
Scan and activate with PID. 
Lock the mapping after activation. 
Use the QR as the permanent digital identity of that property. 
This is scalable because you don't need to know beforehand which PVC plate goes to which house. The association is created only during field activation.

---


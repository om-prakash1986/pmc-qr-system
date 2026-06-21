import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'services/api_service.dart';
import 'theme.dart';
import 'screens/login_screen.dart';
import 'screens/home_screen.dart';
import 'screens/scan_screen.dart';
import 'screens/landing_scan_screen.dart';
import 'screens/citizen_portal_screen.dart';
import 'screens/dashboard_screen.dart';
import 'screens/activation_screen.dart';
import 'screens/success_screen.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  runApp(
    ChangeNotifierProvider(
      create: (_) => ApiService(),
      child: const PmcActivationApp(),
    ),
  );
}

class PmcActivationApp extends StatelessWidget {
  const PmcActivationApp({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'PMC Smart QR Activation',
      theme: PmcTheme.lightTheme,
      debugShowCheckedModeBanner: false,
      home: Consumer<ApiService>(
        builder: (context, api, child) {
          return api.isAuthenticated ? const HomeScreen() : const LandingScanScreen();
        },
      ),
      routes: {
        '/landing_scan': (context) => const LandingScanScreen(),
        '/login': (context) => const LoginScreen(),
        '/home': (context) => const HomeScreen(),
        '/scan': (context) => const ScanScreen(),
        '/activate_form': (context) => const ActivationScreen(),
        '/success': (context) => const SuccessScreen(),
        '/citizen_portal': (context) => CitizenPortalScreen(),
        '/dashboard': (context) => DashboardScreen(),
      },
    );
  }
}

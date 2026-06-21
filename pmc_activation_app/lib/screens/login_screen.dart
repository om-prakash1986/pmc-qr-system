import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:google_fonts/google_fonts.dart';
import '../services/api_service.dart';
import '../theme.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({Key? key}) : super(key: key);

  @override
  _LoginScreenState createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _formKey = GlobalKey<FormState>();
  final _usernameController = TextEditingController();
  final _passwordController = TextEditingController();
  bool _obscurePassword = true;

  @override
  void dispose() {
    _usernameController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  void _submitLogin() async {
    if (!_formKey.currentState!.validate()) return;

    final apiService = Provider.of<ApiService>(context, listen: false);

    try {
      final success = await apiService.login(
        _usernameController.text,
        _passwordController.text,
      );

      if (success) {
        // Capture both passed arguments (from scan page) and raw URL query params
        final routeArgs = ModalRoute.of(context)?.settings.arguments;
        final Map<String, String> mergedParams = {};
        
        if (routeArgs is Map<String, String>) {
          mergedParams.addAll(routeArgs);
        } else if (routeArgs is Map<String, dynamic>) {
          routeArgs.forEach((k, v) => mergedParams[k] = v.toString());
        }
        mergedParams.addAll(Uri.base.queryParameters);

        Navigator.pushReplacementNamed(context, '/home', arguments: mergedParams);
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(e.toString().replaceAll('Exception: ', '')),
          backgroundColor: PmcTheme.dangerRed,
          behavior: SnackBarBehavior.floating,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(12),
          ),
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final apiService = Provider.of<ApiService>(context);
    final size = MediaQuery.of(context).size;

    return Scaffold(
      body: Stack(
        children: [
          // Background Gradient
          Container(
            width: double.infinity,
            height: double.infinity,
            decoration: const BoxDecoration(
              gradient: LinearGradient(
                colors: [PmcTheme.primaryBlue, Color(0xFF1E3E62)],
                begin: Alignment.topCenter,
                end: Alignment.bottomCenter,
              ),
            ),
          ),

          // Decorative circles
          Positioned(
            top: -100,
            right: -100,
            child: Container(
              width: 300,
              height: 300,
              decoration: BoxDecoration(
                shape: BoxShape.circle,
                color: Colors.white.withOpacity(0.05),
              ),
            ),
          ),
          Positioned(
            bottom: -50,
            left: -50,
            child: Container(
              width: 250,
              height: 250,
              decoration: BoxDecoration(
                shape: BoxShape.circle,
                color: PmcTheme.secondaryOrange.withOpacity(0.05),
              ),
            ),
          ),

          Center(
            child: SingleChildScrollView(
              padding: const EdgeInsets.symmetric(horizontal: 24.0),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  // Dual Brand Logos Header
                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      // Patna Nagar Nigam (PMC) Badge
                      _buildLogoBadge(
                        imagePath: 'assets/pmc_logo.jpg',
                        label: 'PMC',
                        sublabel: 'PATNA',
                        primaryColor: PmcTheme.primaryBlue,
                        accentColor: PmcTheme.secondaryOrange,
                      ),
                      const SizedBox(width: 12),
                      Text(
                        '+',
                        style: GoogleFonts.outfit(
                          fontSize: 22,
                          fontWeight: FontWeight.bold,
                          color: Colors.white,
                        ),
                      ),
                      const SizedBox(width: 12),
                      // Indian Bank Badge
                      _buildLogoBadge(
                        imagePath: 'assets/indian_bank.jpg',
                        label: 'INDIAN',
                        sublabel: 'BANK',
                        primaryColor: const Color(
                          0xFF0054A6,
                        ), // Indian Bank Corporate Blue
                        accentColor: const Color(
                          0xFFFFCC00,
                        ), // Indian Bank Corporate Gold
                      ),
                    ],
                  ),
                  const SizedBox(height: 28),

                  // Joint Project Header
                  Text(
                    'DIGITAL IDENTITY PARTNERSHIP',
                    style: GoogleFonts.outfit(
                      fontSize: 12,
                      fontWeight: FontWeight.w700,
                      color: PmcTheme.secondaryOrange,
                      letterSpacing: 2.0,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    'Smart QR Card Activation',
                    style: GoogleFonts.outfit(
                      fontSize: 24,
                      fontWeight: FontWeight.bold,
                      color: Colors.white,
                      letterSpacing: 0.5,
                    ),
                  ),
                  const SizedBox(height: 6),
                  Text(
                    'Patna Nagar Nigam & Indian Bank Joint Initiative',
                    style: GoogleFonts.outfit(
                      fontSize: 13,
                      color: Colors.white.withOpacity(0.7),
                      fontWeight: FontWeight.w500,
                    ),
                    textAlign: TextAlign.center,
                  ),
                  const SizedBox(height: 32),

                  // Glassmorphic Login Card
                  Container(
                    padding: const EdgeInsets.all(28.0),
                    decoration: PmcTheme.glassBoxDecoration(context),
                    child: Form(
                      key: _formKey,
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.stretch,
                        children: [
                          Text(
                            'Staff Login',
                            style: Theme.of(context).textTheme.titleLarge
                                ?.copyWith(color: PmcTheme.primaryBlue),
                            textAlign: TextAlign.center,
                          ),
                          const SizedBox(height: 24),

                          // Username Field
                          TextFormField(
                            controller: _usernameController,
                            keyboardType: TextInputType.text,
                            textInputAction: TextInputAction.next,
                            style: GoogleFonts.outfit(color: PmcTheme.textDark),
                            decoration: const InputDecoration(
                              labelText: 'Employee ID / Login ID',
                              prefixIcon: Icon(
                                Icons.person_outline,
                                color: PmcTheme.primaryBlue,
                              ),
                              hintText: 'e.g. emp101 or admin',
                            ),
                            validator: (value) {
                              if (value == null || value.trim().isEmpty) {
                                return 'Please enter your Employee ID';
                              }
                              return null;
                            },
                          ),
                          const SizedBox(height: 20),

                          // Password Field
                          TextFormField(
                            controller: _passwordController,
                            obscureText: _obscurePassword,
                            textInputAction: TextInputAction.done,
                            style: GoogleFonts.outfit(color: PmcTheme.textDark),
                            onFieldSubmitted: (_) => _submitLogin(),
                            decoration: InputDecoration(
                              labelText: 'Password',
                              prefixIcon: const Icon(
                                Icons.lock_outline,
                                color: PmcTheme.primaryBlue,
                              ),
                              suffixIcon: IconButton(
                                icon: Icon(
                                  _obscurePassword
                                      ? Icons.visibility_outlined
                                      : Icons.visibility_off_outlined,
                                  color: PmcTheme.primaryBlue,
                                ),
                                onPressed: () {
                                  setState(() {
                                    _obscurePassword = !_obscurePassword;
                                  });
                                },
                              ),
                            ),
                            validator: (value) {
                              if (value == null || value.isEmpty) {
                                return 'Please enter your password';
                              }
                              return null;
                            },
                          ),
                          const SizedBox(height: 28),

                          // Login Button
                          ElevatedButton(
                            onPressed: apiService.isLoading
                                ? null
                                : _submitLogin,
                            style: ElevatedButton.styleFrom(
                              backgroundColor: PmcTheme.primaryBlue,
                              padding: const EdgeInsets.symmetric(vertical: 16),
                            ),
                            child: apiService.isLoading
                                ? const SizedBox(
                                    height: 20,
                                    width: 20,
                                    child: CircularProgressIndicator(
                                      color: Colors.white,
                                      strokeWidth: 2,
                                    ),
                                  )
                                : const Text('SECURE LOGIN'),
                          ),
                          const SizedBox(height: 14),

                          // Citizen Portal Redirect Button
                          OutlinedButton(
                            onPressed: () {
                              Navigator.pushNamed(context, '/citizen_portal');
                            },
                            style: OutlinedButton.styleFrom(
                              side: const BorderSide(
                                color: PmcTheme.primaryBlue,
                                width: 1.5,
                              ),
                              padding: const EdgeInsets.symmetric(vertical: 14),
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(16),
                              ),
                            ),
                            child: Text(
                              '🌐 ACCESS CITIZEN PORTAL DEMO',
                              style: GoogleFonts.outfit(
                                fontSize: 13,
                                fontWeight: FontWeight.bold,
                                color: PmcTheme.primaryBlue,
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                  const SizedBox(height: 24),

                  // Toggle Mode Info
                  GestureDetector(
                    onDoubleTap: () {
                      apiService.setSimulated(!apiService.isSimulated);
                      ScaffoldMessenger.of(context).showSnackBar(
                        SnackBar(
                          content: Text(
                            apiService.isSimulated
                                ? 'Switched to Simulated Database Mode'
                                : 'Switched to Live API Database Mode',
                          ),
                          backgroundColor: PmcTheme.secondaryOrange,
                          duration: const Duration(seconds: 1),
                        ),
                      );
                    },
                    child: Text(
                      apiService.isSimulated
                          ? '⚙️ Simulated Mode Active (Double-tap to change)'
                          : '🔌 Live Server Mode Active: ${apiService.baseUrl}',
                      style: GoogleFonts.outfit(
                        fontSize: 12,
                        color: Colors.white.withOpacity(0.5),
                      ),
                      textAlign: TextAlign.center,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildLogoBadge({
    required String imagePath,
    required String label,
    required String sublabel,
    required Color primaryColor,
    required Color accentColor,
  }) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.08),
            blurRadius: 10,
            offset: const Offset(0, 4),
          ),
        ],
        border: Border.all(color: primaryColor.withOpacity(0.15), width: 1),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Container(
            width: 32,
            height: 32,
            decoration: BoxDecoration(
              color: Colors.white,
              shape: BoxShape.circle,
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.05),
                  blurRadius: 4,
                  offset: const Offset(0, 1),
                ),
              ],
            ),
            child: ClipRRect(
              borderRadius: BorderRadius.circular(16),
              child: Image.asset(
                imagePath,
                fit: BoxFit.contain,
                errorBuilder: (context, error, stackTrace) {
                  return Icon(
                    Icons.broken_image_rounded,
                    size: 16,
                    color: primaryColor,
                  );
                },
              ),
            ),
          ),
          const SizedBox(width: 8),
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                label,
                style: GoogleFonts.outfit(
                  fontSize: 11,
                  fontWeight: FontWeight.bold,
                  color: primaryColor,
                  letterSpacing: 0.5,
                  height: 1.1,
                ),
              ),
              Text(
                sublabel,
                style: GoogleFonts.outfit(
                  fontSize: 9.5,
                  fontWeight: FontWeight.w600,
                  color: accentColor,
                  height: 1.1,
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

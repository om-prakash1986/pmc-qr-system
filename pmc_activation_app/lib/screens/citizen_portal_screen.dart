import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:provider/provider.dart';
import '../theme.dart';
import '../services/api_service.dart';
import '../models/property.dart';
import '../models/qr_card.dart';

class CitizenPortalScreen extends StatefulWidget {
  final String? initialToken;
  const CitizenPortalScreen({Key? key, this.initialToken}) : super(key: key);

  @override
  _CitizenPortalScreenState createState() => _CitizenPortalScreenState();
}

class _CitizenPortalScreenState extends State<CitizenPortalScreen>
    with TickerProviderStateMixin {
  final _tokenController = TextEditingController();
  final _mobileController = TextEditingController();
  final List<TextEditingController> _otpControllers = List.generate(
    6,
    (_) => TextEditingController(),
  );
  final List<FocusNode> _otpFocusNodes = List.generate(6, (_) => FocusNode());

  // Flow State
  String _currentStep = 'token'; // 'token', 'mobile', 'otp', 'dashboard'
  bool _isLoading = false;
  String? _errorMessage;

  // Data retrieved
  QrCard? _scannedCard;
  Property? _property;
  String _simulatedOtp = '123456';
  bool _isCardFlipped = false;

  // Animation controllers
  late AnimationController _confettiController;
  bool _showConfetti = false;

  @override
  void initState() {
    super.initState();
    _confettiController = AnimationController(
      vsync: this,
      duration: const Duration(seconds: 2),
    );

    String? token = widget.initialToken;
    if (token == null || token.isEmpty) {
      token =
          Uri.base.queryParameters['token'] ??
          Uri.base.queryParameters['value'];
      if (token == null || token.isEmpty) {
        final href = Uri.base.toString();
        if (href.contains('token=')) {
          token = href.split('token=').last.split('&').first;
        } else if (href.contains('value=')) {
          token = href.split('value=').last.split('&').first;
        }
      }
    }

    if (token != null && token.isNotEmpty) {
      _tokenController.text = token;
      // Auto look up token
      WidgetsBinding.instance.addPostFrameCallback((_) {
        _verifyCardToken();
      });
    }
  }

  @override
  void dispose() {
    _tokenController.dispose();
    _mobileController.dispose();
    for (var controller in _otpControllers) {
      controller.dispose();
    }
    for (var node in _otpFocusNodes) {
      node.dispose();
    }
    _confettiController.dispose();
    super.dispose();
  }

  void _verifyCardToken() async {
    final token = _tokenController.text.trim();
    if (token.isEmpty) {
      setState(() => _errorMessage = 'Please enter a QR Token or URL');
      return;
    }

    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    final apiService = Provider.of<ApiService>(context, listen: false);

    try {
      // Look up card details
      final card = await apiService.lookupCard(token);

      if (!card.isActivated) {
        throw Exception(
          'This QR card is UNASSIGNED. Patna Nagar Nigam staff has not linked it to any Property ID (PID) yet.',
        );
      }

      // Pre-fetch property to show masked mobile number
      final property = await apiService.lookupProperty(card.propertyId);

      setState(() {
        _scannedCard = card;
        _property = property;
        _isLoading = false;
        _currentStep = 'mobile';
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
        _errorMessage = e.toString().replaceAll('Exception: ', '');
      });
    }
  }

  void _sendOtp() async {
    final mobile = _mobileController.text.trim();
    if (mobile.length != 10 || double.tryParse(mobile) == null) {
      setState(
        () => _errorMessage = 'Please enter a valid 10-digit mobile number',
      );
      return;
    }

    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    final apiService = Provider.of<ApiService>(context, listen: false);

    try {
      // Use pre-fetched property to check registered mobile
      final prop = _property!;

      // Verification check (allow 9431881414 as override for simulator usability)
      if (prop.mobileNo != mobile && mobile != '9431881414') {
        throw Exception(
          'The mobile number entered is not registered with this property. Registered mobile ends in: ${prop.mobileNo.substring(prop.mobileNo.length - 4)}',
        );
      }

      // Generate random simulated OTP
      _simulatedOtp = (100000 + (DateTime.now().millisecond * 900) % 900000)
          .toInt()
          .toString();

      setState(() {
        _isLoading = false;
        _currentStep = 'otp';
      });

      // Show Toast / Dialog displaying the simulated OTP for testing
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            '⚡ DEMO SMS: Verification OTP for your property is $_simulatedOtp',
          ),
          backgroundColor: PmcTheme.secondaryOrange,
          duration: const Duration(seconds: 4),
          action: SnackBarAction(
            label: 'AUTOFILL',
            textColor: Colors.white,
            onPressed: () {
              for (int i = 0; i < 6; i++) {
                _otpControllers[i].text = _simulatedOtp[i];
              }
              _verifyOtp();
            },
          ),
        ),
      );
    } catch (e) {
      setState(() {
        _isLoading = false;
        _errorMessage = e.toString().replaceAll('Exception: ', '');
      });
    }
  }

  void _verifyOtp() {
    String enteredOtp = '';
    for (var controller in _otpControllers) {
      enteredOtp += controller.text.trim();
    }

    if (enteredOtp.length != 6) {
      setState(
        () => _errorMessage =
            'Please enter all 6 digits of the verification code',
      );
      return;
    }

    if (enteredOtp != _simulatedOtp && enteredOtp != '123456') {
      setState(
        () => _errorMessage = 'Invalid verification code. Please try again.',
      );
      return;
    }

    setState(() {
      _currentStep = 'dashboard';
      _errorMessage = null;
      _showConfetti = true;
    });

    _confettiController.forward(from: 0.0).then((_) {
      setState(() {
        _showConfetti = false;
      });
    });
  }

  void _resetFlow() {
    setState(() {
      _currentStep = 'token';
      _scannedCard = null;
      _property = null;
      _mobileController.clear();
      _errorMessage = null;
      for (var controller in _otpControllers) {
        controller.clear();
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    // Read optional token passed via routing
    final routeArgs =
        ModalRoute.of(context)?.settings.arguments as Map<String, dynamic>?;
    String? token = routeArgs != null && routeArgs.containsKey('token')
        ? routeArgs['token']
        : null;
    if ((token == null || token.isEmpty) && _tokenController.text.isEmpty) {
      token =
          Uri.base.queryParameters['token'] ??
          Uri.base.queryParameters['value'];
      if (token == null || token.isEmpty) {
        final href = Uri.base.toString();
        if (href.contains('token=')) {
          token = href.split('token=').last.split('&').first;
        } else if (href.contains('value=')) {
          token = href.split('value=').last.split('&').first;
        }
      }
    }
    if (token != null && token.isNotEmpty && _tokenController.text.isEmpty) {
      _tokenController.text = token;
      WidgetsBinding.instance.addPostFrameCallback((_) {
        _verifyCardToken();
      });
    }

    return Scaffold(
      backgroundColor: const Color(0xFF0B192C), // Dark theme for citizen portal
      appBar: AppBar(
        title: Text(
          'Resident Smart QR Portal',
          style: GoogleFonts.outfit(
            fontWeight: FontWeight.bold,
            color: Colors.white,
            fontSize: 18,
          ),
        ),
        centerTitle: true,
        backgroundColor: PmcTheme.primaryBlue,
        iconTheme: const IconThemeData(color: Colors.white),
        actions: [
          if (_currentStep != 'token')
            IconButton(
              icon: const Icon(Icons.refresh_rounded),
              onPressed: _resetFlow,
            ),
        ],
      ),
      body: Stack(
        children: [
          // Background Glows
          Positioned(
            top: -150,
            left: -150,
            child: Container(
              width: 350,
              height: 350,
              decoration: BoxDecoration(
                shape: BoxShape.circle,
                color: PmcTheme.primaryBlue.withOpacity(0.15),
              ),
            ),
          ),
          Positioned(
            bottom: -100,
            right: -100,
            child: Container(
              width: 300,
              height: 300,
              decoration: BoxDecoration(
                shape: BoxShape.circle,
                color: PmcTheme.secondaryOrange.withOpacity(0.08),
              ),
            ),
          ),

          // Main Content View
          Center(
            child: SingleChildScrollView(
              padding: const EdgeInsets.all(20.0),
              child: ConstrainedBox(
                constraints: const BoxConstraints(maxWidth: 460),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    // Dual Logos branding
                    _buildBrandingHeader(),
                    const SizedBox(height: 20),

                    // Error banner
                    if (_errorMessage != null) _buildErrorBanner(),

                    // Multi-step form container (glassmorphic styling)
                    Container(
                      padding: const EdgeInsets.all(24.0),
                      decoration: BoxDecoration(
                        color: Colors.white.withOpacity(0.06),
                        borderRadius: BorderRadius.circular(24),
                        border: Border.all(
                          color: Colors.white.withOpacity(0.12),
                        ),
                      ),
                      child: _buildCurrentStepWidget(),
                    ),
                    const SizedBox(height: 20),

                    // Instructions helper
                    if (_currentStep == 'token') _buildHelpPanel(),
                  ],
                ),
              ),
            ),
          ),

          // Celebrate Confetti Overlay
          if (_showConfetti)
            IgnorePointer(
              child: AnimatedBuilder(
                animation: _confettiController,
                builder: (context, child) {
                  return CustomPaint(
                    size: Size.infinite,
                    painter: ConfettiPainter(_confettiController.value),
                  );
                },
              ),
            ),
        ],
      ),
    );
  }

  Widget _buildBrandingHeader() {
    return Column(
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Container(
              padding: const EdgeInsets.all(6),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(10),
              ),
              child: Image.asset(
                'assets/pmc_logo.jpg',
                width: 32,
                height: 32,
                errorBuilder: (_, __, ___) =>
                    const Icon(Icons.home, color: PmcTheme.primaryBlue),
              ),
            ),
            const SizedBox(width: 8),
            Text(
              '+',
              style: GoogleFonts.outfit(
                color: PmcTheme.secondaryOrange,
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(width: 8),
            Container(
              padding: const EdgeInsets.all(6),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(10),
              ),
              child: Image.asset(
                'assets/indian_bank.jpg',
                width: 32,
                height: 32,
                errorBuilder: (_, __, ___) =>
                    const Icon(Icons.account_balance, color: Colors.blue),
              ),
            ),
          ],
        ),
        const SizedBox(height: 12),
        Text(
          'PATNA NAGAR NIGAM',
          style: GoogleFonts.outfit(
            color: PmcTheme.secondaryOrange,
            fontWeight: FontWeight.bold,
            fontSize: 10,
            letterSpacing: 2.5,
          ),
        ),
        const SizedBox(height: 4),
        Text(
          _currentStep == 'dashboard'
              ? 'Smart Address Card Verified'
              : 'Resident Portal Login',
          style: GoogleFonts.outfit(
            color: Colors.white,
            fontSize: 20,
            fontWeight: FontWeight.bold,
          ),
        ),
      ],
    );
  }

  Widget _buildErrorBanner() {
    return Container(
      margin: const EdgeInsets.only(bottom: 20),
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: PmcTheme.dangerRed.withOpacity(0.12),
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: PmcTheme.dangerRed.withOpacity(0.3)),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Icon(Icons.error_outline_rounded, color: Colors.redAccent),
          const SizedBox(width: 10),
          Expanded(
            child: Text(
              _errorMessage!,
              style: GoogleFonts.outfit(
                color: Colors.red.shade200,
                fontSize: 13,
                height: 1.4,
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCurrentStepWidget() {
    switch (_currentStep) {
      case 'token':
        return _buildTokenStep();
      case 'mobile':
        return _buildMobileStep();
      case 'otp':
        return _buildOtpStep();
      case 'dashboard':
        return _buildDashboardStep();
      default:
        return Container();
    }
  }

  Widget _buildTokenStep() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(
          'Card Verification',
          style: GoogleFonts.outfit(
            color: Colors.white,
            fontWeight: FontWeight.bold,
            fontSize: 15,
          ),
          textAlign: TextAlign.center,
        ),
        const SizedBox(height: 16),
        TextField(
          controller: _tokenController,
          style: GoogleFonts.outfit(color: Colors.white),
          decoration: InputDecoration(
            labelText: 'QR ID or Token Value',
            hintText: 'e.g. 5A9F9999-BB99-99AA-AA99-AA9999999999',
            prefixIcon: const Icon(
              Icons.qr_code_2_rounded,
              color: PmcTheme.secondaryOrange,
            ),
            labelStyle: const TextStyle(color: PmcTheme.textDarkSecondary),
            filled: true,
            fillColor: Colors.white.withOpacity(0.04),
            enabledBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(16),
              borderSide: BorderSide(color: Colors.white.withOpacity(0.12)),
            ),
            focusedBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(16),
              borderSide: const BorderSide(color: PmcTheme.secondaryOrange),
            ),
          ),
        ),
        const SizedBox(height: 20),
        ElevatedButton(
          onPressed: _isLoading ? null : _verifyCardToken,
          style: ElevatedButton.styleFrom(
            backgroundColor: PmcTheme.secondaryOrange,
          ),
          child: _isLoading
              ? const SizedBox(
                  width: 20,
                  height: 20,
                  child: CircularProgressIndicator(
                    color: Colors.white,
                    strokeWidth: 2,
                  ),
                )
              : const Text('VERIFY SMART CARD'),
        ),
      ],
    );
  }

  Widget _buildMobileStep() {
    final apiService = Provider.of<ApiService>(context);
    String last4 = '****';
    if (_property != null && _property!.mobileNo.length >= 4) {
      last4 = _property!.mobileNo.substring(_property!.mobileNo.length - 4);
    }

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Container(
          padding: const EdgeInsets.all(12),
          decoration: BoxDecoration(
            color: PmcTheme.secondaryOrange.withOpacity(0.12),
            borderRadius: BorderRadius.circular(14),
            border: Border.all(
              color: PmcTheme.secondaryOrange.withOpacity(0.25),
            ),
          ),
          child: Row(
            children: [
              const Icon(
                Icons.lock_person_rounded,
                color: PmcTheme.secondaryOrange,
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Text(
                  'Verification Required:\nLinked registered mobile ends in: ****$last4',
                  style: GoogleFonts.outfit(
                    color: Colors.white.withOpacity(0.9),
                    fontSize: 12.5,
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 20),
        TextField(
          controller: _mobileController,
          keyboardType: TextInputType.phone,
          maxLength: 10,
          style: GoogleFonts.outfit(color: Colors.white),
          decoration: InputDecoration(
            labelText: 'Registered Mobile Number',
            hintText: 'Enter 10-digit mobile number',
            prefixIcon: const Icon(
              Icons.phone_android_rounded,
              color: PmcTheme.secondaryOrange,
            ),
            counterText: '',
            labelStyle: const TextStyle(color: PmcTheme.textDarkSecondary),
            filled: true,
            fillColor: Colors.white.withOpacity(0.04),
            enabledBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(16),
              borderSide: BorderSide(color: Colors.white.withOpacity(0.12)),
            ),
            focusedBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(16),
              borderSide: const BorderSide(color: PmcTheme.secondaryOrange),
            ),
          ),
        ),
        if (apiService.isSimulated) ...[
          const SizedBox(height: 10),
          GestureDetector(
            onTap: () {
              _mobileController.text = _scannedCard!.propertyId == '1409113'
                  ? '9431881414'
                  : '9199774991';
            },
            child: Text(
              '💡 Demo Auto-Fill: ${_scannedCard!.propertyId == "1409113" ? "9431881414" : "9199774991"} (Tap)',
              style: GoogleFonts.outfit(
                color: Colors.yellow.shade600,
                fontSize: 11,
                fontWeight: FontWeight.bold,
              ),
              textAlign: TextAlign.right,
            ),
          ),
        ],
        const SizedBox(height: 20),
        ElevatedButton(
          onPressed: _isLoading ? null : _sendOtp,
          style: ElevatedButton.styleFrom(
            backgroundColor: PmcTheme.secondaryOrange,
          ),
          child: _isLoading
              ? const SizedBox(
                  width: 20,
                  height: 20,
                  child: CircularProgressIndicator(
                    color: Colors.white,
                    strokeWidth: 2,
                  ),
                )
              : const Text('GET VERIFICATION CODE'),
        ),
      ],
    );
  }

  Widget _buildOtpStep() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(
          'Verification Code Sent',
          style: GoogleFonts.outfit(
            color: Colors.white,
            fontWeight: FontWeight.bold,
            fontSize: 15,
          ),
          textAlign: TextAlign.center,
        ),
        const SizedBox(height: 8),
        Text(
          'We have simulated/sent a 6-digit OTP to your registered mobile ending in: ${_mobileController.text.substring(6)}',
          style: GoogleFonts.outfit(
            color: PmcTheme.textDarkSecondary,
            fontSize: 12,
          ),
          textAlign: TextAlign.center,
        ),
        const SizedBox(height: 20),

        // 6 input boxes
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: List.generate(6, (index) {
            return SizedBox(
              width: 45,
              height: 50,
              child: TextFormField(
                controller: _otpControllers[index],
                focusNode: _otpFocusNodes[index],
                keyboardType: TextInputType.number,
                textAlign: TextAlign.center,
                maxLength: 1,
                style: GoogleFonts.outfit(
                  color: Colors.white,
                  fontSize: 18,
                  fontWeight: FontWeight.bold,
                ),
                decoration: InputDecoration(
                  counterText: '',
                  contentPadding: EdgeInsets.zero,
                  filled: true,
                  fillColor: Colors.white.withOpacity(0.04),
                  enabledBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(10),
                    borderSide: BorderSide(
                      color: Colors.white.withOpacity(0.12),
                    ),
                  ),
                  focusedBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(10),
                    borderSide: const BorderSide(
                      color: PmcTheme.secondaryOrange,
                    ),
                  ),
                ),
                onChanged: (value) {
                  if (value.length == 1) {
                    if (index < 5) {
                      _otpFocusNodes[index + 1].requestFocus();
                    } else {
                      _otpFocusNodes[index].unfocus();
                      _verifyOtp();
                    }
                  } else if (value.isEmpty) {
                    if (index > 0) {
                      _otpFocusNodes[index - 1].requestFocus();
                    }
                  }
                },
              ),
            );
          }),
        ),
        const SizedBox(height: 24),
        ElevatedButton(
          onPressed: _verifyOtp,
          style: ElevatedButton.styleFrom(
            backgroundColor: PmcTheme.secondaryOrange,
          ),
          child: const Text('VERIFY & ACCESS DETAILS'),
        ),
        const SizedBox(height: 12),
        TextButton(
          onPressed: _sendOtp,
          child: Text(
            'RESEND OTP',
            style: GoogleFonts.outfit(
              color: PmcTheme.secondaryOrange,
              fontWeight: FontWeight.bold,
              fontSize: 13,
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildDashboardStep() {
    final duesAmt = double.tryParse(_property!.totalDues) ?? 0.0;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        // Certified header banner
        Row(
          children: [
            const Icon(
              Icons.verified_user_rounded,
              color: PmcTheme.successGreen,
              size: 24,
            ),
            const SizedBox(width: 8),
            Text(
              'Sparrow Record Verified',
              style: GoogleFonts.outfit(
                color: PmcTheme.successGreen,
                fontWeight: FontWeight.bold,
                fontSize: 14,
              ),
            ),
          ],
        ),
        const SizedBox(height: 16),
        const Divider(color: Colors.white12, height: 1),
        const SizedBox(height: 16),

        // Outstanding Dues card
        Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: duesAmt > 0
                ? PmcTheme.secondaryOrange.withOpacity(0.08)
                : PmcTheme.successGreen.withOpacity(0.08),
            borderRadius: BorderRadius.circular(18),
            border: Border.all(
              color: duesAmt > 0
                  ? PmcTheme.secondaryOrange.withOpacity(0.25)
                  : PmcTheme.successGreen.withOpacity(0.25),
              width: 1.5,
            ),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    'Municipal Dues Status',
                    style: GoogleFonts.outfit(
                      color: PmcTheme.textDarkSecondary,
                      fontSize: 12,
                    ),
                  ),
                  Container(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 10,
                      vertical: 4,
                    ),
                    decoration: BoxDecoration(
                      color: duesAmt > 0
                          ? PmcTheme.dangerRed.withOpacity(0.15)
                          : PmcTheme.successGreen.withOpacity(0.15),
                      borderRadius: BorderRadius.circular(6),
                    ),
                    child: Text(
                      _property!.paymentStatus.toUpperCase(),
                      style: GoogleFonts.outfit(
                        color: duesAmt > 0
                            ? PmcTheme.dangerRed
                            : PmcTheme.successGreen,
                        fontWeight: FontWeight.bold,
                        fontSize: 10,
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 8),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    '₹${_property!.totalDues}',
                    style: GoogleFonts.outfit(
                      color: duesAmt > 0
                          ? PmcTheme.secondaryOrange
                          : PmcTheme.successGreen,
                      fontSize: 22,
                      fontWeight: FontWeight.w800,
                    ),
                  ),
                  if (duesAmt > 0)
                    ElevatedButton(
                      onPressed: () => _showRazorpayCheckout(duesAmt),
                      style: ElevatedButton.styleFrom(
                        padding: const EdgeInsets.symmetric(
                          horizontal: 12,
                          vertical: 6,
                        ),
                        minimumSize: Size.zero,
                        tapTargetSize: MaterialTapTargetSize.shrinkWrap,
                        backgroundColor: PmcTheme.secondaryOrange,
                      ),
                      child: const Text(
                        'PAY TAX',
                        style: TextStyle(fontSize: 11),
                      ),
                    ),
                ],
              ),
            ],
          ),
        ),
        const SizedBox(height: 18),

        // Core Property Fields
        _buildPropertyRow(
          'Property ID (PID)',
          _property!.pid,
          isHighlighted: true,
        ),
        _buildPropertyRow(
          'Registered Owner',
          _property!.ownerName,
          valueColor: PmcTheme.secondaryOrange,
        ),
        _buildPropertyRow('Father/Guardian', _property!.guardianName),
        _buildPropertyRow('Holding Address', _property!.address),

        Row(
          children: [
            Expanded(child: _buildPropertyRow('Ward No', _property!.wardNo)),
            const SizedBox(width: 10),
            Expanded(
              child: _buildPropertyRow('Revenue Circle', _property!.circle),
            ),
          ],
        ),
        const SizedBox(height: 12),
        const Divider(color: Colors.white12, height: 1),
        const SizedBox(height: 16),

        // Flip interactive PVC Card
        Text(
          'Digital Smart Identity Card (Tap to flip)',
          style: GoogleFonts.outfit(
            color: PmcTheme.textDarkSecondary,
            fontSize: 11,
            fontWeight: FontWeight.w600,
          ),
          textAlign: TextAlign.center,
        ),
        const SizedBox(height: 10),
        GestureDetector(
          onTap: () {
            setState(() {
              _isCardFlipped = !_isCardFlipped;
            });
          },
          child: AnimatedContainer(
            duration: const Duration(milliseconds: 300),
            height: 180,
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(18),
              gradient: LinearGradient(
                colors: _isCardFlipped
                    ? [const Color(0xFF1E3E62), const Color(0xFF0D3E73)]
                    : [PmcTheme.primaryBlue, const Color(0xFF09223D)],
                begin: Alignment.topLeft,
                end: Alignment.bottomRight,
              ),
              border: Border.all(color: Colors.white24, width: 1.5),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.35),
                  blurRadius: 12,
                  offset: const Offset(0, 6),
                ),
              ],
            ),
            padding: const EdgeInsets.all(16),
            child: _isCardFlipped ? _buildPvcCardBack() : _buildPvcCardFront(),
          ),
        ),

        const SizedBox(height: 24),
        OutlinedButton.icon(
          onPressed: _resetFlow,
          icon: const Icon(Icons.logout_rounded, color: Colors.white70),
          label: const Text(
            'LOGOUT & CLOSE',
            style: TextStyle(color: Colors.white70),
          ),
          style: OutlinedButton.styleFrom(
            side: const BorderSide(color: Colors.white24),
            padding: const EdgeInsets.symmetric(vertical: 14),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(16),
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildPvcCardFront() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              'PATNA NAGAR NIGAM',
              style: GoogleFonts.outfit(
                color: Colors.white,
                fontWeight: FontWeight.bold,
                fontSize: 11,
                letterSpacing: 1.0,
              ),
            ),
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
              decoration: BoxDecoration(
                border: Border.all(color: PmcTheme.secondaryOrange, width: 1.2),
                borderRadius: BorderRadius.circular(4),
              ),
              child: const Text(
                'SMART CARD',
                style: TextStyle(
                  color: PmcTheme.secondaryOrange,
                  fontSize: 8,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
          ],
        ),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'PROPERTY ID CARD',
                    style: TextStyle(color: Colors.white38, fontSize: 8),
                  ),
                  const SizedBox(height: 2),
                  Text(
                    _property!.ownerName,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: GoogleFonts.outfit(
                      color: Colors.white,
                      fontWeight: FontWeight.bold,
                      fontSize: 13,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    'PID: ${_property!.pid}',
                    style: GoogleFonts.outfit(
                      color: PmcTheme.secondaryOrange,
                      fontSize: 11,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Text(
                    'QR ID: ${_scannedCard!.qrId}',
                    style: const TextStyle(color: Colors.white54, fontSize: 9),
                  ),
                ],
              ),
            ),
            Container(
              padding: const EdgeInsets.all(4),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(6),
              ),
              child: const Icon(
                Icons.qr_code_2_rounded,
                color: Colors.black,
                size: 48,
              ),
            ),
          ],
        ),
        const Text(
          'Smart Tax Identity of Patna Nagar Nigam',
          style: TextStyle(
            color: Colors.white24,
            fontSize: 7,
            letterSpacing: 0.5,
          ),
          textAlign: TextAlign.center,
        ),
      ],
    );
  }

  Widget _buildPvcCardBack() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Icon(
              Icons.contact_support_outlined,
              color: PmcTheme.secondaryOrange,
              size: 16,
            ),
            Text(
              'INSTRUCTIONS',
              style: GoogleFonts.outfit(
                color: Colors.white60,
                fontWeight: FontWeight.bold,
                fontSize: 10,
                letterSpacing: 1.0,
              ),
            ),
          ],
        ),
        const SizedBox(height: 8),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              _buildInstructionRow(
                '1. Keep the QR plate clean for quick scanning.',
              ),
              _buildInstructionRow(
                '2. Scan with default camera to view tax dues instantly.',
              ),
              _buildInstructionRow(
                '3. Do not scratch or relocate the plate without supervisor approval.',
              ),
            ],
          ),
        ),
        const Divider(color: Colors.white12, height: 1),
        const SizedBox(height: 4),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: const [
            Text(
              'Helpline: 1800-XXX-XXXX',
              style: TextStyle(color: Colors.white30, fontSize: 8),
            ),
            Text(
              'website: pmc.bihar.gov.in',
              style: TextStyle(color: Colors.white30, fontSize: 8),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildInstructionRow(String text) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 2.0),
      child: Text(
        text,
        style: GoogleFonts.outfit(color: Colors.white70, fontSize: 9.5),
      ),
    );
  }

  Widget _buildPropertyRow(
    String label,
    String value, {
    bool isHighlighted = false,
    Color? valueColor,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            label,
            style: GoogleFonts.outfit(
              fontSize: 10,
              color: PmcTheme.textDarkSecondary,
            ),
          ),
          const SizedBox(height: 2),
          Text(
            value,
            style: GoogleFonts.outfit(
              fontSize: isHighlighted ? 15 : 13,
              fontWeight: isHighlighted ? FontWeight.bold : FontWeight.w500,
              color:
                  valueColor ??
                  (isHighlighted ? const Color(0xFF54A0FF) : Colors.white),
            ),
          ),
        ],
      ),
    );
  }

  void _showRazorpayCheckout(double duesAmt) {
    showDialog(
      context: context,
      barrierDismissible: false, // Prevents closing during transaction
      builder: (BuildContext dialogContext) {
        String activeTab = 'card'; // 'card', 'upi', 'netbanking'
        String transactionState = 'input'; // 'input', 'processing', 'success'
        String mockPaymentId =
            'pay_rzp_mock_${DateTime.now().millisecondsSinceEpoch.toString().substring(5)}';

        final cardNoController = TextEditingController(
          text: '4111 1111 1111 1111',
        );
        final expiryController = TextEditingController(text: '12/29');
        final cvvController = TextEditingController(text: '123');
        final nameController = TextEditingController(
          text: _property?.ownerName ?? 'SRI KUMUD VISHAL',
        );
        final upiController = TextEditingController(text: 'citizen@okaxis');
        String selectedBank = 'Indian Bank';

        return StatefulBuilder(
          builder: (context, setDialogState) {
            if (transactionState == 'processing') {
              return Dialog(
                backgroundColor: const Color(0xFF0F172A),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                ),
                child: Padding(
                  padding: const EdgeInsets.all(28.0),
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      const CircularProgressIndicator(color: Color(0xFF3393FF)),
                      const SizedBox(height: 24),
                      Text(
                        'Processing Payment...',
                        style: GoogleFonts.outfit(
                          color: Colors.white,
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const SizedBox(height: 8),
                      Text(
                        'Connecting securely to Razorpay gateway.\nDo not press back or close this window.',
                        style: GoogleFonts.outfit(
                          color: Colors.white60,
                          fontSize: 12,
                        ),
                        textAlign: TextAlign.center,
                      ),
                    ],
                  ),
                ),
              );
            }

            if (transactionState == 'success') {
              return Dialog(
                backgroundColor: Colors.white,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Container(
                  width: double.infinity,
                  constraints: const BoxConstraints(maxWidth: 400),
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      // Header
                      Container(
                        padding: const EdgeInsets.symmetric(vertical: 20),
                        decoration: const BoxDecoration(
                          color: Color(0xFF0D3E73),
                          borderRadius: BorderRadius.only(
                            topLeft: Radius.circular(12),
                            topRight: Radius.circular(12),
                          ),
                        ),
                        child: Column(
                          children: [
                            const Icon(Icons.verified, color: Colors.greenAccent, size: 48),
                            const SizedBox(height: 10),
                            Text(
                              'PAYMENT SUCCESSFUL',
                              style: GoogleFonts.outfit(
                                color: Colors.white,
                                fontSize: 18,
                                fontWeight: FontWeight.bold,
                                letterSpacing: 1.5,
                              ),
                            ),
                          ],
                        ),
                      ),
                      
                      // Body
                      Padding(
                        padding: const EdgeInsets.all(24.0),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Center(
                              child: Text(
                                'PATNA NAGAR NIGAM',
                                style: GoogleFonts.outfit(
                                  color: Colors.black87,
                                  fontSize: 16,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            ),
                            const SizedBox(height: 4),
                            Center(
                              child: Text(
                                'Municipal Tax Receipt',
                                style: GoogleFonts.outfit(
                                  color: Colors.black54,
                                  fontSize: 12,
                                ),
                              ),
                            ),
                            const Divider(height: 30, thickness: 1, color: Colors.black12),
                            
                            _buildReceiptRow('Transaction ID', mockPaymentId),
                            const SizedBox(height: 10),
                            _buildReceiptRow('Date & Time', DateTime.now().toString().substring(0, 16)),
                            const SizedBox(height: 10),
                            _buildReceiptRow('Property ID', _property?.pid ?? ''),
                            const SizedBox(height: 10),
                            _buildReceiptRow('Owner Name', _property?.ownerName ?? ''),
                            const SizedBox(height: 10),
                            _buildReceiptRow('Payment Mode', 'Online Payment'),
                            
                            const Divider(height: 30, thickness: 1, color: Colors.black12),
                            
                            Row(
                              mainAxisAlignment: MainAxisAlignment.spaceBetween,
                              children: [
                                Text(
                                  'Amount Paid',
                                  style: GoogleFonts.outfit(
                                    color: Colors.black87,
                                    fontSize: 16,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                                Text(
                                  '₹${duesAmt.toStringAsFixed(2)}',
                                  style: GoogleFonts.outfit(
                                    color: const Color(0xFF10B981),
                                    fontSize: 18,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                              ],
                            ),
                          ],
                        ),
                      ),
                      
                      // Actions
                      Padding(
                        padding: const EdgeInsets.only(bottom: 24, left: 24, right: 24),
                        child: Row(
                          children: [
                            Expanded(
                              child: OutlinedButton.icon(
                                style: OutlinedButton.styleFrom(
                                  padding: const EdgeInsets.symmetric(vertical: 12),
                                  side: const BorderSide(color: Color(0xFF0D3E73)),
                                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
                                ),
                                icon: const Icon(Icons.download, color: Color(0xFF0D3E73), size: 18),
                                label: Text(
                                  'SAVE',
                                  style: GoogleFonts.outfit(color: const Color(0xFF0D3E73), fontWeight: FontWeight.bold),
                                ),
                                onPressed: () {
                                  ScaffoldMessenger.of(context).showSnackBar(
                                    const SnackBar(content: Text('Receipt downloaded to gallery.')),
                                  );
                                },
                              ),
                            ),
                            const SizedBox(width: 12),
                            Expanded(
                              child: ElevatedButton(
                                style: ElevatedButton.styleFrom(
                                  backgroundColor: const Color(0xFF0D3E73),
                                  padding: const EdgeInsets.symmetric(vertical: 12),
                                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
                                ),
                                child: Text(
                                  'DONE',
                                  style: GoogleFonts.outfit(color: Colors.white, fontWeight: FontWeight.bold),
                                ),
                                onPressed: () {
                                  Navigator.of(dialogContext).pop();
                                  // Clear dues
                                  setState(() {
                                    _property = Property(
                                      pid: _property!.pid,
                                      ownerName: _property!.ownerName,
                                      guardianName: _property!.guardianName,
                                      mobileNo: _property!.mobileNo,
                                      address: _property!.address,
                                      totalDues: '0.00',
                                      paymentStatus: 'Paid',
                                      circle: _property!.circle,
                                      revenueCircleNo: _property!.revenueCircleNo,
                                      wardNo: _property!.wardNo,
                                    );
                                    _showConfetti = true;
                                  });
                                  _confettiController.forward(from: 0.0).then((_) {
                                    setState(() {
                                      _showConfetti = false;
                                    });
                                  });
                                  ScaffoldMessenger.of(context).showSnackBar(
                                    SnackBar(
                                      content: Text(
                                        '🎉 Payment of ₹${duesAmt.toStringAsFixed(2)} processed successfully!',
                                      ),
                                      backgroundColor: Colors.green,
                                    ),
                                  );
                                },
                              ),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              );
            }

            return Dialog(
              backgroundColor: const Color(0xFF0F172A),
              insetPadding: const EdgeInsets.symmetric(
                horizontal: 16,
                vertical: 24,
              ),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(16),
              ),
              child: ConstrainedBox(
                constraints: const BoxConstraints(maxWidth: 400),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    // Header
                    Container(
                      padding: const EdgeInsets.all(16),
                      decoration: const BoxDecoration(
                        color: Color(0xFF1E293B),
                        borderRadius: BorderRadius.only(
                          topLeft: Radius.circular(16),
                          topRight: Radius.circular(16),
                        ),
                      ),
                      child: Row(
                        children: [
                          Container(
                            padding: const EdgeInsets.all(6),
                            decoration: BoxDecoration(
                              color: Colors.white,
                              borderRadius: BorderRadius.circular(8),
                            ),
                            child: const Icon(
                              Icons.account_balance,
                              color: Color(0xFF0D3E73),
                              size: 24,
                            ),
                          ),
                          const SizedBox(width: 12),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  'Patna Nagar Nigam',
                                  style: GoogleFonts.outfit(
                                    color: Colors.white,
                                    fontSize: 14,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                                Text(
                                  'Municipal Tax Payment',
                                  style: GoogleFonts.outfit(
                                    color: Colors.white60,
                                    fontSize: 11,
                                  ),
                                ),
                              ],
                            ),
                          ),
                          Column(
                            crossAxisAlignment: CrossAxisAlignment.end,
                            children: [
                              Text(
                                '₹${duesAmt.toStringAsFixed(2)}',
                                style: GoogleFonts.outfit(
                                  color: Colors.white,
                                  fontSize: 16,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              Text(
                                'rzp_test_mode',
                                style: GoogleFonts.outfit(
                                  color: Colors.orange,
                                  fontSize: 9,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            ],
                          ),
                          IconButton(
                            icon: const Icon(
                              Icons.close,
                              color: Colors.white60,
                              size: 20,
                            ),
                            onPressed: () => Navigator.of(dialogContext).pop(),
                          ),
                        ],
                      ),
                    ),

                    // Tabs Header
                    Container(
                      color: const Color(0xFF1E293B).withOpacity(0.5),
                      child: Row(
                        children: [
                          _buildTabButton(
                            title: 'Cards',
                            icon: Icons.credit_card,
                            isActive: activeTab == 'card',
                            onTap: () =>
                                setDialogState(() => activeTab = 'card'),
                          ),
                          _buildTabButton(
                            title: 'UPI',
                            icon: Icons.qr_code,
                            isActive: activeTab == 'upi',
                            onTap: () =>
                                setDialogState(() => activeTab = 'upi'),
                          ),
                          _buildTabButton(
                            title: 'Net Banking',
                            icon: Icons.account_balance_wallet,
                            isActive: activeTab == 'netbanking',
                            onTap: () =>
                                setDialogState(() => activeTab = 'netbanking'),
                          ),
                        ],
                      ),
                    ),

                    // Tab Content
                    Padding(
                      padding: const EdgeInsets.all(16.0),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.stretch,
                        children: [
                          if (activeTab == 'card') ...[
                            _buildTextField(
                              'Card Number',
                              cardNoController,
                              hint: '4111 1111 1111 1111',
                            ),
                            const SizedBox(height: 12),
                            Row(
                              children: [
                                Expanded(
                                  child: _buildTextField(
                                    'Expiry',
                                    expiryController,
                                    hint: 'MM/YY',
                                  ),
                                ),
                                const SizedBox(width: 12),
                                Expanded(
                                  child: _buildTextField(
                                    'CVV',
                                    cvvController,
                                    hint: '123',
                                    obscureText: true,
                                  ),
                                ),
                              ],
                            ),
                            const SizedBox(height: 12),
                            _buildTextField(
                              'Cardholder Name',
                              nameController,
                              hint: 'Name on card',
                            ),
                          ] else if (activeTab == 'upi') ...[
                            _buildTextField(
                              'UPI ID / VPA',
                              upiController,
                              hint: 'username@bank',
                            ),
                            const SizedBox(height: 16),
                            Row(
                              children: [
                                const Icon(
                                  Icons.flash_on,
                                  color: Colors.amber,
                                  size: 16,
                                ),
                                const SizedBox(width: 8),
                                Expanded(
                                  child: Text(
                                    'Keep your UPI app ready (PhonePe, GPay, Paytm) to approve request.',
                                    style: GoogleFonts.outfit(
                                      color: Colors.white60,
                                      fontSize: 11,
                                    ),
                                  ),
                                ),
                              ],
                            ),
                          ] else if (activeTab == 'netbanking') ...[
                            Text(
                              'Select Popular Bank',
                              style: GoogleFonts.outfit(
                                color: Colors.white70,
                                fontSize: 12,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                            const SizedBox(height: 12),
                            Wrap(
                              spacing: 8,
                              runSpacing: 8,
                              children:
                                  [
                                    'Indian Bank',
                                    'State Bank of India',
                                    'HDFC Bank',
                                    'ICICI Bank',
                                    'Axis Bank',
                                  ].map((bank) {
                                    final isSelected = selectedBank == bank;
                                    return ChoiceChip(
                                      label: Text(
                                        bank,
                                        style: GoogleFonts.outfit(
                                          color: isSelected
                                              ? Colors.white
                                              : Colors.white60,
                                          fontSize: 11,
                                        ),
                                      ),
                                      selected: isSelected,
                                      selectedColor: const Color(0xFF3393FF),
                                      backgroundColor: const Color(0xFF1E293B),
                                      onSelected: (selected) {
                                        if (selected) {
                                          setDialogState(
                                            () => selectedBank = bank,
                                          );
                                        }
                                      },
                                    );
                                  }).toList(),
                            ),
                          ],

                          const SizedBox(height: 24),

                          // Pay Button
                          ElevatedButton(
                            style: ElevatedButton.styleFrom(
                              backgroundColor: const Color(0xFF3393FF),
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(10),
                              ),
                              padding: const EdgeInsets.symmetric(vertical: 16),
                            ),
                            onPressed: () async {
                              setDialogState(() {
                                transactionState = 'processing';
                              });
                              try {
                                final apiService = Provider.of<ApiService>(context, listen: false);
                                await apiService.processPayment(
                                  _property!.pid,
                                  duesAmt,
                                  mockPaymentId,
                                  ownerName: _property!.ownerName,
                                  mobileNo: _property!.mobileNo,
                                );
                                if (mounted) {
                                  setDialogState(() {
                                    transactionState = 'success';
                                  });
                                }
                              } catch (e) {
                                if (mounted) {
                                  Navigator.of(dialogContext).pop();
                                  ScaffoldMessenger.of(context).showSnackBar(
                                    SnackBar(
                                      content: Text(e.toString()),
                                      backgroundColor: Colors.red,
                                    ),
                                  );
                                }
                              }
                            },
                            child: Text(
                              'PAY ₹${duesAmt.toStringAsFixed(2)}',
                              style: GoogleFonts.outfit(
                                color: Colors.white,
                                fontSize: 14,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ),

                          const SizedBox(height: 16),

                          // Footer logo
                          Row(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              const Icon(
                                Icons.security,
                                color: Colors.white30,
                                size: 14,
                              ),
                              const SizedBox(width: 6),
                              Text(
                                'Secured by Razorpay checkout',
                                style: GoogleFonts.outfit(
                                  color: Colors.white30,
                                  fontSize: 10,
                                ),
                              ),
                            ],
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            );
          },
        );
      },
    );
  }

  Widget _buildTabButton({
    required String title,
    required IconData icon,
    required bool isActive,
    required VoidCallback onTap,
  }) {
    return Expanded(
      child: InkWell(
        onTap: onTap,
        child: Container(
          padding: const EdgeInsets.symmetric(vertical: 12),
          decoration: BoxDecoration(
            border: Border(
              bottom: BorderSide(
                color: isActive ? const Color(0xFF3393FF) : Colors.transparent,
                width: 2,
              ),
            ),
          ),
          child: Column(
            children: [
              Icon(
                icon,
                color: isActive ? const Color(0xFF3393FF) : Colors.white60,
                size: 18,
              ),
              const SizedBox(height: 4),
              Text(
                title,
                style: GoogleFonts.outfit(
                  color: isActive ? Colors.white : Colors.white60,
                  fontSize: 10,
                  fontWeight: isActive ? FontWeight.bold : FontWeight.normal,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildReceiptRow(String label, String value) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        SizedBox(
          width: 120,
          child: Text(
            label,
            style: GoogleFonts.outfit(color: Colors.black54, fontSize: 13),
          ),
        ),
        const Text(':  ', style: TextStyle(color: Colors.black54)),
        Expanded(
          child: Text(
            value,
            style: GoogleFonts.outfit(
              color: Colors.black87,
              fontSize: 14,
              fontWeight: FontWeight.w600,
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildTextField(
    String label,
    TextEditingController controller, {
    String hint = '',
    bool obscureText = false,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: GoogleFonts.outfit(color: Colors.white70, fontSize: 11),
        ),
        const SizedBox(height: 6),
        SizedBox(
          height: 45,
          child: TextField(
            controller: controller,
            obscureText: obscureText,
            style: GoogleFonts.outfit(color: Colors.white, fontSize: 13),
            decoration: InputDecoration(
              hintText: hint,
              hintStyle: const TextStyle(color: Colors.white30),
              contentPadding: const EdgeInsets.symmetric(
                horizontal: 12,
                vertical: 8,
              ),
              filled: true,
              fillColor: const Color(0xFF1E293B),
              enabledBorder: OutlineInputBorder(
                borderRadius: BorderRadius.circular(8),
                borderSide: const BorderSide(color: Colors.white10),
              ),
              focusedBorder: OutlineInputBorder(
                borderRadius: BorderRadius.circular(8),
                borderSide: const BorderSide(color: Color(0xFF3393FF)),
              ),
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildHelpPanel() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white.withOpacity(0.03),
        borderRadius: BorderRadius.circular(18),
        border: Border.all(color: Colors.white.withOpacity(0.06)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Row(
            children: [
              const Icon(
                Icons.info_outline_rounded,
                color: PmcTheme.secondaryOrange,
                size: 20,
              ),
              const SizedBox(width: 8),
              Text(
                'How to Scan QR',
                style: GoogleFonts.outfit(
                  color: Colors.white,
                  fontWeight: FontWeight.bold,
                  fontSize: 13,
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Text(
            'In the real world, residents scan the QR plate on their wall using their phone camera, which opens the web login page directly.\n\nHere, you can enter any activated card token (e.g. 5A9F9999-BB99-99AA-AA99-AA9999999999) to simulate the portal experience.',
            style: GoogleFonts.outfit(
              color: PmcTheme.textDarkSecondary,
              fontSize: 11.5,
              height: 1.4,
            ),
          ),
        ],
      ),
    );
  }
}

// Confetti Particle Custom Painter for celebration
class ConfettiPainter extends CustomPainter {
  final double animationValue;
  ConfettiPainter(this.animationValue);

  @override
  void paint(Canvas canvas, Size size) {
    final rand = _RandomSeeder(12345);
    final paint = Paint()..style = PaintingStyle.fill;
    final colors = [
      PmcTheme.secondaryOrange,
      PmcTheme.successGreen,
      PmcTheme.primaryBlue,
      Colors.pinkAccent,
      Colors.purpleAccent,
      Colors.yellowAccent,
    ];

    for (int i = 0; i < 80; i++) {
      final color = colors[rand.nextInt(colors.length)];
      final speed = rand.nextDouble() * 200 + 150;
      final startX = rand.nextDouble() * size.width;

      // Fall down y direction based on animation value
      final y = (animationValue * speed) % size.height;
      final x = startX + (y / (rand.nextDouble() * 3 + 1));
      final sizeRadius = rand.nextDouble() * 4 + 3;

      paint.color = color;
      canvas.drawRect(
        Rect.fromCenter(
          center: Offset(x % size.width, y),
          width: sizeRadius * 1.5,
          height: sizeRadius,
        ),
        paint,
      );
    }
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => true;
}

class _RandomSeeder {
  int seed;
  _RandomSeeder(this.seed);

  int nextInt(int max) {
    seed = (seed * 1103515245 + 12345) & 0x7fffffff;
    return seed % max;
  }

  double nextDouble() {
    return nextInt(10000) / 10000.0;
  }
}

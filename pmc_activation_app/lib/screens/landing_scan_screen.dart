import 'dart:async';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:mobile_scanner/mobile_scanner.dart';
import '../services/api_service.dart';
import '../theme.dart';

class LandingScanScreen extends StatefulWidget {
  const LandingScanScreen({Key? key}) : super(key: key);

  @override
  _LandingScanScreenState createState() => _LandingScanScreenState();
}

class _LandingScanScreenState extends State<LandingScanScreen>
    with SingleTickerProviderStateMixin {
  final _manualController = TextEditingController();
  late AnimationController _animationController;
  late Animation<double> _laserPosition;
  bool _isProcessing = false;
  String? _errorMessage;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      vsync: this,
      duration: const Duration(seconds: 2),
    )..repeat(reverse: true);

    _laserPosition = Tween<double>(begin: 0.0, end: 220.0).animate(
      CurvedAnimation(parent: _animationController, curve: Curves.easeInOut),
    );
  }

  @override
  void dispose() {
    _animationController.dispose();
    _manualController.dispose();
    super.dispose();
  }

  void _handleScanResult(String scannedValue) {
    if (_isProcessing) return;

    final Map<String, String> payload = ApiService.extractPayload(scannedValue);

    // Immediately route to Login Screen with the payload
    Navigator.pushNamed(context, '/login', arguments: payload);
  }

  @override
  Widget build(BuildContext context) {
    final apiService = Provider.of<ApiService>(context);



    return Scaffold(
      backgroundColor: PmcTheme.backgroundLight,
      appBar: AppBar(
        title: Text(
          'Tax Collector QR Scanner',
          style: GoogleFonts.outfit(
            fontWeight: FontWeight.bold,
            color: Colors.white,
            fontSize: 18,
          ),
        ),
        centerTitle: true,
        backgroundColor: PmcTheme.primaryBlue,
        iconTheme: const IconThemeData(color: Colors.white),
      ),
      body: SingleChildScrollView(
        child: Padding(
          padding: const EdgeInsets.all(24.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              // 1. Instructions Header
              Text(
                'Please scan the PVC QR Code installed at the property.',
                style: GoogleFonts.outfit(
                  fontSize: 16,
                  fontWeight: FontWeight.w500,
                  color: PmcTheme.textDark,
                ),
                textAlign: TextAlign.center,
              ),
              const SizedBox(height: 24),

              // 2. Beautiful Animated Viewport
              Center(
                child: Container(
                  width: 250,
                  height: 250,
                  decoration: BoxDecoration(
                    color: Colors.black.withOpacity(0.05),
                    borderRadius: BorderRadius.circular(24),
                    border: Border.all(color: PmcTheme.primaryBlue, width: 2),
                  ),
                  child: Stack(
                    children: [
                      // Real Camera Scanner
                      ClipRRect(
                        borderRadius: BorderRadius.circular(22),
                        child: MobileScanner(
                          controller: MobileScannerController(
                            detectionSpeed: DetectionSpeed.noDuplicates,
                            facing: CameraFacing.back,
                          ),
                          onDetect: (capture) {
                            if (_isProcessing) return;
                            final List<Barcode> barcodes = capture.barcodes;
                            if (barcodes.isNotEmpty) {
                              final barcode = barcodes.first;
                              if (barcode.rawValue != null) {
                                _handleScanResult(barcode.rawValue!);
                              }
                            }
                          },
                        ),
                      ),
                      // Viewfinder border indicators (corners)
                      _buildCornerBorder(top: 10, left: 10, angle: 0),
                      _buildCornerBorder(top: 10, right: 10, angle: 1.57),
                      _buildCornerBorder(bottom: 10, left: 10, angle: 4.71),
                      _buildCornerBorder(bottom: 10, right: 10, angle: 3.14),

                      // Laser scanner line
                      AnimatedBuilder(
                        animation: _laserPosition,
                        builder: (context, child) {
                          return Positioned(
                            top: 15 + _laserPosition.value,
                            left: 15,
                            right: 15,
                            child: Container(
                              height: 3,
                              decoration: BoxDecoration(
                                color: PmcTheme.secondaryOrange,
                                boxShadow: [
                                  BoxShadow(
                                    color: PmcTheme.secondaryOrange.withOpacity(0.8),
                                    blurRadius: 8,
                                    spreadRadius: 2,
                                  ),
                                ],
                              ),
                            ),
                          );
                        },
                      ),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 30),

              // 3. Manual Text Input Form
              Card(
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                ),
                elevation: 4,
                child: Padding(
                  padding: const EdgeInsets.all(18.0),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      Text(
                        'Manual PID/Token Entry',
                        style: GoogleFonts.outfit(
                          fontSize: 14,
                          fontWeight: FontWeight.bold,
                          color: PmcTheme.primaryBlue,
                        ),
                      ),
                      const SizedBox(height: 12),
                      Row(
                        children: [
                          Expanded(
                            child: TextField(
                              controller: _manualController,
                              style: GoogleFonts.outfit(
                                color: PmcTheme.textDark,
                                fontSize: 14,
                              ),
                              decoration: InputDecoration(
                                hintText: 'Enter PID or QR Token',
                                contentPadding: const EdgeInsets.symmetric(
                                  horizontal: 16,
                                  vertical: 12,
                                ),
                                filled: true,
                                fillColor: Colors.grey.shade100,
                                border: OutlineInputBorder(
                                  borderRadius: BorderRadius.circular(12),
                                  borderSide: BorderSide.none,
                                ),
                              ),
                            ),
                          ),
                          const SizedBox(width: 10),
                          ElevatedButton(
                            onPressed: _isProcessing
                                ? null
                                : () {
                                    if (_manualController.text.trim().isNotEmpty) {
                                      _handleScanResult(_manualController.text.trim());
                                    }
                                  },
                            style: ElevatedButton.styleFrom(
                              backgroundColor: PmcTheme.secondaryOrange,
                              padding: const EdgeInsets.symmetric(
                                horizontal: 18,
                                vertical: 14,
                              ),
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(12),
                              ),
                            ),
                            child: const Icon(Icons.arrow_forward_rounded, color: Colors.white),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 20),

              // 4. Skip Login Button
              TextButton.icon(
                onPressed: () {
                  Navigator.pushNamed(context, '/login');
                },
                icon: const Icon(Icons.login_rounded, color: PmcTheme.primaryBlue),
                label: Text(
                  'Skip Scan and Go to Login',
                  style: GoogleFonts.outfit(
                    fontWeight: FontWeight.bold,
                    fontSize: 16,
                    color: PmcTheme.primaryBlue,
                  ),
                ),
              ),
              const SizedBox(height: 10),


            ],
          ),
        ),
      ),
    );
  }

  Widget _buildCornerBorder({
    double? top,
    double? bottom,
    double? left,
    double? right,
    required double angle,
  }) {
    return Positioned(
      top: top,
      bottom: bottom,
      left: left,
      right: right,
      child: Transform.rotate(
        angle: angle,
        child: Container(
          width: 30,
          height: 30,
          decoration: const BoxDecoration(
            border: Border(
              top: BorderSide(color: PmcTheme.primaryBlue, width: 4),
              left: BorderSide(color: PmcTheme.primaryBlue, width: 4),
            ),
          ),
        ),
      ),
    );
  }
}

import 'dart:async';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:mobile_scanner/mobile_scanner.dart';
import '../services/api_service.dart';
import '../theme.dart';
import '../models/qr_card.dart';

class ScanScreen extends StatefulWidget {
  const ScanScreen({Key? key}) : super(key: key);

  @override
  _ScanScreenState createState() => _ScanScreenState();
}

class _ScanScreenState extends State<ScanScreen>
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

  void _handleScanResult(String scannedValue) async {
    if (_isProcessing) return;

    setState(() {
      _isProcessing = true;
      _errorMessage = null;
    });

    final apiService = Provider.of<ApiService>(context, listen: false);
    final parsedValue = ApiService.extractPayload(scannedValue)['qrId'] ?? scannedValue;

    try {
      // Look up QR Card details in DB/API
      final qrCard = await apiService.lookupCard(parsedValue);

      setState(() {
        _isProcessing = false;
      });

      // Route to Activation Form Screen
      Navigator.pushNamed(context, '/activate_form', arguments: qrCard);
    } catch (e) {
      setState(() {
        _isProcessing = false;
        _errorMessage = e.toString().replaceAll('Exception: ', '');
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final apiService = Provider.of<ApiService>(context);

    return Scaffold(
      appBar: AppBar(
        title: Text(
          'Scan QR Code',
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
                'Hold the PVC plate in front of the camera or enter the details manually below.',
                style: GoogleFonts.outfit(
                  fontSize: 15,
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
                                    color: PmcTheme.secondaryOrange.withOpacity(
                                      0.8,
                                    ),
                                    blurRadius: 8,
                                    spreadRadius: 2,
                                  ),
                                ],
                              ),
                            ),
                          );
                        },
                      ),

                      // Loading Overlay
                      if (_isProcessing)
                        Container(
                          decoration: BoxDecoration(
                            color: Colors.white.withOpacity(0.8),
                            borderRadius: BorderRadius.circular(22),
                          ),
                          child: const Center(
                            child: CircularProgressIndicator(
                              color: PmcTheme.primaryBlue,
                            ),
                          ),
                        ),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 28),

              // 3. Error Banner
              if (_errorMessage != null)
                Container(
                  padding: const EdgeInsets.all(12),
                  margin: const EdgeInsets.only(bottom: 20),
                  decoration: BoxDecoration(
                    color: PmcTheme.dangerRed.withOpacity(0.1),
                    borderRadius: BorderRadius.circular(12),
                    border: Border.all(
                      color: PmcTheme.dangerRed.withOpacity(0.3),
                    ),
                  ),
                  child: Row(
                    children: [
                      const Icon(
                        Icons.error_outline_rounded,
                        color: PmcTheme.dangerRed,
                      ),
                      const SizedBox(width: 10),
                      Expanded(
                        child: Text(
                          _errorMessage!,
                          style: GoogleFonts.outfit(
                            color: PmcTheme.dangerRed,
                            fontSize: 13,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),

              // 4. Manual Text Input Form
              Card(
                child: Padding(
                  padding: const EdgeInsets.all(18.0),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      Text(
                        'Manual Registration Entry',
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
                              decoration: const InputDecoration(
                                hintText: 'Enter QRToken, ID or full URL',
                                contentPadding: EdgeInsets.symmetric(
                                  horizontal: 16,
                                  vertical: 12,
                                ),
                              ),
                            ),
                          ),
                          const SizedBox(width: 8),
                          ElevatedButton(
                            onPressed: _isProcessing
                                ? null
                                : () {
                                    if (_manualController.text
                                        .trim()
                                        .isNotEmpty) {
                                      _handleScanResult(
                                        _manualController.text.trim(),
                                      );
                                    }
                                  },
                            style: ElevatedButton.styleFrom(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 16,
                                vertical: 12,
                              ),
                            ),
                            child: const Icon(Icons.arrow_forward_rounded),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 20),


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

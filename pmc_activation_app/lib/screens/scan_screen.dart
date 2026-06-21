import 'dart:async';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:google_fonts/google_fonts.dart';
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

    try {
      // Look up QR Card details in DB/API
      final qrCard = await apiService.lookupCard(scannedValue);

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

    // List of mock cards available for quick click testing in simulated mode
    final testCards = [
      {
        'label': 'Test Card 1 (Unassigned)',
        'token': 'CEF733F3-7416-4FFA-87F4-2D4361DA8791',
        'url':
            'https://pmc.bihar.gov.in/qr/CEF733F3-7416-4FFA-87F4-2D4361DA8791',
        'qrid': 'PMC/IND/DQR/00457417',
      },
      {
        'label': 'Test Card 2 (Unassigned)',
        'token': '4F23CAC1-2D0E-406A-A0DB-54300D5C46EB',
        'url':
            'https://pmc.bihar.gov.in/qr/4F23CAC1-2D0E-406A-A0DB-54300D5C46EB',
        'qrid': 'PMC/IND/DQR/00457418',
      },
      {
        'label': 'Test Card 3 (Unassigned)',
        'token': 'BF388527-E492-4176-AC8E-0042633BADB3',
        'url':
            'https://pmc.bihar.gov.in/qr/BF388527-E492-4176-AC8E-0042633BADB3',
        'qrid': 'PMC/IND/DQR/00457419',
      },
      {
        'label': 'Activated Card (Security Block Test)',
        'token': '5A9F9999-BB99-99AA-AA99-AA9999999999',
        'url':
            'https://pmc.bihar.gov.in/qr/5A9F9999-BB99-99AA-AA99-AA9999999999',
        'qrid': 'PMC/IND/DQR/00004567',
      },
    ];

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
                      // Viewfinder border indicators (corners)
                      _buildCornerBorder(top: 10, left: 10, angle: 0),
                      _buildCornerBorder(top: 10, right: 10, angle: 1.57),
                      _buildCornerBorder(bottom: 10, left: 10, angle: 4.71),
                      _buildCornerBorder(bottom: 10, right: 10, angle: 3.14),

                      // QR Code placeholder icon in background
                      Center(
                        child: Icon(
                          Icons.qr_code_2_rounded,
                          size: 140,
                          color: Colors.grey.withOpacity(0.2),
                        ),
                      ),

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

              // 5. Simulated QR Code Quick selector (Tapped triggers scanning lookup)
              if (apiService.isSimulated) ...[
                Text(
                  'Simulation Quick Tap (Tap to simulate scan)',
                  style: GoogleFonts.outfit(
                    fontSize: 13,
                    fontWeight: FontWeight.bold,
                    color: PmcTheme.textLight,
                  ),
                ),
                const SizedBox(height: 10),
                ...testCards.map((card) {
                  return Card(
                    margin: const EdgeInsets.only(bottom: 8),
                    child: ListTile(
                      dense: true,
                      leading: const Icon(
                        Icons.qr_code_rounded,
                        color: PmcTheme.primaryBlue,
                      ),
                      title: Text(
                        card['label']!,
                        style: GoogleFonts.outfit(
                          fontWeight: FontWeight.w600,
                          color: PmcTheme.textDark,
                        ),
                      ),
                      subtitle: Text(
                        'Token: ${card['token']!.substring(0, 8)}... | ID: ${card['qrid']}',
                        style: GoogleFonts.outfit(fontSize: 11),
                      ),
                      trailing: const Icon(
                        Icons.tap_and_play_rounded,
                        color: PmcTheme.secondaryOrange,
                      ),
                      onTap: _isProcessing
                          ? null
                          : () => _handleScanResult(card['url']!),
                    ),
                  );
                }).toList(),
              ],
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

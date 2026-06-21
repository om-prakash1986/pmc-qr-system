import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import '../theme.dart';

class SuccessScreen extends StatelessWidget {
  const SuccessScreen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final args =
        ModalRoute.of(context)!.settings.arguments as Map<String, String>;
    final qrId = args['qrId'] ?? '';
    final pid = args['pid'] ?? '';
    final ownerName = args['ownerName'] ?? '';
    final address = args['address'] ?? '';

    return Scaffold(
      backgroundColor: Colors.white,
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 24.0, vertical: 20),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const Spacer(),

              // 1. Success Visual Badge (Tick)
              Center(
                child: Container(
                  width: 100,
                  height: 100,
                  decoration: BoxDecoration(
                    color: PmcTheme.successGreen.withOpacity(0.12),
                    shape: BoxShape.circle,
                  ),
                  child: const Center(
                    child: Icon(
                      Icons.verified_rounded,
                      size: 68,
                      color: PmcTheme.successGreen,
                    ),
                  ),
                ),
              ),
              const SizedBox(height: 24),

              // 2. Titles
              Text(
                'Linking Complete!',
                style: GoogleFonts.outfit(
                  fontSize: 24,
                  fontWeight: FontWeight.bold,
                  color: PmcTheme.primaryBlue,
                ),
                textAlign: TextAlign.center,
              ),
              const SizedBox(height: 6),
              Text(
                'The QR plate has been permanently linked to the property database.',
                style: GoogleFonts.outfit(
                  fontSize: 14,
                  color: PmcTheme.textLight,
                ),
                textAlign: TextAlign.center,
              ),
              const SizedBox(height: 40),

              // 3. Confirmation Details card
              Card(
                elevation: 3,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(20),
                  side: BorderSide(color: Colors.grey.shade200, width: 1),
                ),
                child: Padding(
                  padding: const EdgeInsets.all(22.0),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'Mapping Summary',
                        style: GoogleFonts.outfit(
                          fontSize: 13,
                          fontWeight: FontWeight.bold,
                          color: PmcTheme.textLight,
                          letterSpacing: 0.5,
                        ),
                      ),
                      const SizedBox(height: 16),
                      const Divider(height: 1),
                      const SizedBox(height: 16),
                      _buildSummaryRow(
                        Icons.qr_code_scanner_rounded,
                        'QR ID',
                        qrId,
                      ),
                      _buildSummaryRow(
                        Icons.home_work_rounded,
                        'Property ID (PID)',
                        pid,
                      ),
                      _buildSummaryRow(
                        Icons.person_rounded,
                        'Owner Name',
                        ownerName,
                      ),
                      _buildSummaryRow(
                        Icons.location_on_rounded,
                        'Site Address',
                        address,
                      ),
                    ],
                  ),
                ),
              ),

              const Spacer(),

              // 4. Return Trigger Button
              ElevatedButton(
                onPressed: () {
                  Navigator.pushReplacementNamed(context, '/home');
                },
                style: ElevatedButton.styleFrom(
                  backgroundColor: PmcTheme.primaryBlue,
                  padding: const EdgeInsets.symmetric(vertical: 18),
                ),
                child: Text(
                  'RETURN TO DASHBOARD',
                  style: GoogleFonts.outfit(
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                    letterSpacing: 0.5,
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

  Widget _buildSummaryRow(IconData icon, String label, String value) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 16.0),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, color: PmcTheme.primaryBlue, size: 22),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  label,
                  style: GoogleFonts.outfit(
                    color: PmcTheme.textLight,
                    fontSize: 11,
                  ),
                ),
                const SizedBox(height: 2),
                Text(
                  value,
                  style: GoogleFonts.outfit(
                    color: PmcTheme.textDark,
                    fontWeight: FontWeight.bold,
                    fontSize: 14.5,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

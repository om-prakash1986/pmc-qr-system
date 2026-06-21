import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import '../models/dashboard_models.dart';
import '../theme.dart';

class EventTile extends StatelessWidget {
  final ActivationEvent event;
  const EventTile({Key? key, required this.event}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Row(
          children: [
            const Icon(Icons.qr_code, color: PmcTheme.successGreen, size: 28),
            const SizedBox(width: 16),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'PID: ${event.pid}',
                    style: GoogleFonts.outfit(
                      fontWeight: FontWeight.bold,
                      fontSize: 15,
                      color: PmcTheme.textDark,
                    ),
                  ),
                  const SizedBox(height: 2),
                  Text(
                    'QR: ${event.qrId}',
                    style: GoogleFonts.outfit(
                      fontSize: 13,
                      color: PmcTheme.textLight,
                    ),
                  ),
                  if (event.owner != null) ...[
                    const SizedBox(height: 2),
                    Text(
                      event.owner!,
                      style: GoogleFonts.outfit(
                        fontSize: 12,
                        color: PmcTheme.primaryBlue,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ],
                ],
              ),
            ),
            Column(
              crossAxisAlignment: CrossAxisAlignment.end,
              children: const [
                Icon(
                  Icons.verified_rounded,
                  color: PmcTheme.successGreen,
                  size: 18,
                ),
                SizedBox(height: 8),
                // Note: activatedAt will be filled at runtime
              ],
            ),
          ],
        ),
      ),
    );
  }
}

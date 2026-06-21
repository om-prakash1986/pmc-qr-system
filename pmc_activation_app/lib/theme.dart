import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';

class PmcTheme {
  static const Color primaryBlue = Color(0xFF0D3E73);
  static const Color secondaryOrange = Color(0xFFE67E22);
  static const Color successGreen = Color(0xFF2ECC71);
  static const Color dangerRed = Color(0xFFE74C3C);

  static const Color backgroundLight = Color(0xFFF5F7FA);
  static const Color surfaceLight = Colors.white;
  static const Color textDark = Color(0xFF2C3E50);
  static const Color textLight = Color(0xFF7F8C8D);

  static const Color backgroundDark = Color(0xFF0B192C);
  static const Color surfaceDark = Color(0xFF1E3E62);
  static const Color textDarkPrimary = Colors.white;
  static const Color textDarkSecondary = Color(0xFFBDC3C7);

  static ThemeData get lightTheme {
    return ThemeData(
      useMaterial3: true,
      brightness: Brightness.light,
      primaryColor: primaryBlue,
      scaffoldBackgroundColor: backgroundLight,
      colorScheme: const ColorScheme.light(
        primary: primaryBlue,
        secondary: secondaryOrange,
        error: dangerRed,
        surface: surfaceLight,
      ),
      textTheme: GoogleFonts.outfitTextTheme().copyWith(
        titleLarge: GoogleFonts.outfit(
          fontSize: 22,
          fontWeight: FontWeight.bold,
          color: textDark,
        ),
        titleMedium: GoogleFonts.outfit(
          fontSize: 18,
          fontWeight: FontWeight.w600,
          color: textDark,
        ),
        bodyLarge: GoogleFonts.outfit(fontSize: 16, color: textDark),
        bodyMedium: GoogleFonts.outfit(fontSize: 14, color: textLight),
      ),
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: Colors.white,
        contentPadding: const EdgeInsets.symmetric(
          horizontal: 20,
          vertical: 16,
        ),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(16),
          borderSide: BorderSide(color: Colors.grey.shade300, width: 1.5),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(16),
          borderSide: BorderSide(color: Colors.grey.shade300, width: 1.5),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(16),
          borderSide: const BorderSide(color: primaryBlue, width: 2),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(16),
          borderSide: const BorderSide(color: dangerRed, width: 1.5),
        ),
      ),
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          backgroundColor: primaryBlue,
          foregroundColor: Colors.white,
          elevation: 2,
          padding: const EdgeInsets.symmetric(vertical: 16, horizontal: 24),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(16),
          ),
          textStyle: GoogleFonts.outfit(
            fontSize: 16,
            fontWeight: FontWeight.bold,
          ),
        ),
      ),
      cardTheme: CardThemeData(
        color: Colors.white,
        elevation: 3,
        shadowColor: Colors.black.withOpacity(0.05),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
      ),
    );
  }

  static BoxDecoration glassBoxDecoration(BuildContext context) {
    return BoxDecoration(
      color: Colors.white.withOpacity(0.9),
      borderRadius: BorderRadius.circular(24),
      border: Border.all(color: Colors.white.withOpacity(0.2)),
      boxShadow: [
        BoxShadow(
          color: Colors.black.withOpacity(0.05),
          blurRadius: 15,
          offset: const Offset(0, 8),
        ),
      ],
    );
  }
}

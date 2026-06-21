import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';
import 'package:pmc_activation_app/main.dart';
import 'package:pmc_activation_app/services/api_service.dart';

void main() {
  testWidgets('App launch smoke test', (WidgetTester tester) async {
    // Build our app and trigger a frame.
    await tester.pumpWidget(
      ChangeNotifierProvider(
        create: (_) => ApiService(),
        child: const PmcActivationApp(),
      ),
    );

    // Verify that the login screen is loaded by checking for standard titles
    expect(find.text('Staff Login'), findsOneWidget);
    expect(find.text('SECURE LOGIN'), findsOneWidget);
    expect(find.text('PATNA NAGAR NIGAM'), findsOneWidget);
  });
}

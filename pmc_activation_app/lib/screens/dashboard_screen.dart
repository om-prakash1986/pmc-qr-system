import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:provider/provider.dart';
import '../services/dashboard_service.dart';
import '../models/dashboard_models.dart';
import '../widgets/summary_card.dart';
import '../widgets/event_tile.dart';
import '../theme.dart';

class DashboardScreen extends StatefulWidget {
  const DashboardScreen({Key? key}) : super(key: key);

  @override
  State<DashboardScreen> createState() => _DashboardScreenState();
}

class _DashboardScreenState extends State<DashboardScreen> {
  final DashboardService _service = DashboardService();
  bool _loading = true;
  ActivationStats? _stats;
  List<ActivationEvent> _events = [];

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  Future<void> _loadData() async {
    setState(() => _loading = true);
    try {
      final stats = await _service.fetchStats();
      final events = await _service.fetchRecentEvents();
      setState(() {
        _stats = stats;
        _events = events;
      });
    } catch (e) {
      // ignore for now; could show a snackbar
    } finally {
      setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(
          'Activation Dashboard',
          style: GoogleFonts.outfit(fontWeight: FontWeight.bold),
        ),
        backgroundColor: PmcTheme.primaryBlue,
      ),
      body: RefreshIndicator(
        onRefresh: _loadData,
        child: _loading
            ? const Center(child: CircularProgressIndicator())
            : SingleChildScrollView(
                physics: const AlwaysScrollableScrollPhysics(),
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    if (_stats != null) ...[
                      SummaryCard(
                        label: 'Total Activations',
                        value: _stats!.totalActivations.toString(),
                        icon: Icons.check_circle,
                        color: PmcTheme.successGreen,
                      ),
                      const SizedBox(height: 12),
                      SummaryCard(
                        label: 'Successful',
                        value: _stats!.successful.toString(),
                        icon: Icons.thumb_up,
                        color: PmcTheme.successGreen,
                      ),
                      const SizedBox(height: 12),
                      SummaryCard(
                        label: 'Failed',
                        value: _stats!.failed.toString(),
                        icon: Icons.thumb_down,
                        color: PmcTheme.dangerRed,
                      ),
                      const SizedBox(height: 12),
                      SummaryCard(
                        label: 'Pending Payments',
                        value: _stats!.pendingPayments.toString(),
                        icon: Icons.payment,
                        color: PmcTheme.secondaryOrange,
                      ),
                      const SizedBox(height: 24),
                    ],
                    const Text(
                      'Recent Activations',
                      style: TextStyle(
                        fontSize: 18,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: 12),
                    if (_events.isEmpty)
                      const Center(child: Text('No recent activity'))
                    else
                      ListView.builder(
                        shrinkWrap: true,
                        physics: const NeverScrollableScrollPhysics(),
                        itemCount: _events.length,
                        itemBuilder: (context, index) =>
                            EventTile(event: _events[index]),
                      ),
                  ],
                ),
              ),
      ),
    );
  }
}

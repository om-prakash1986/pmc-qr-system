import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:google_fonts/google_fonts.dart';
import '../services/api_service.dart';
import '../theme.dart';
import '../models/property_detail_row.dart';
import '../models/tax_history_row.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({Key? key}) : super(key: key);

  @override
  _HomeScreenState createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  final _searchController = TextEditingController();
  bool _isSearching = false;
  String? _errorMessage;
  List<PropertyDetailRow>? _searchResults;
  List<TaxHistoryRow>? _taxHistoryResults;

  @override
  void initState() {
    super.initState();
    // Automatically load default PID for Tax Collector Demo
    WidgetsBinding.instance.addPostFrameCallback((_) async {
      final apiService = Provider.of<ApiService>(context, listen: false);
      
      // Attempt to read from route arguments (passed from login screen)
      final routeArgs = ModalRoute.of(context)?.settings.arguments;
      final Map<String, String> queryParams = {};
      
      if (routeArgs is Map<String, String>) {
        queryParams.addAll(routeArgs);
      } else if (routeArgs is Map<String, dynamic>) {
        routeArgs.forEach((k, v) => queryParams[k] = v.toString());
      } else {
        queryParams.addAll(Uri.base.queryParameters);
      }
      
      String? targetPid = queryParams['pid'] ?? queryParams['search'];
      String? token = queryParams['token'] ?? queryParams['qrId'] ?? queryParams['value'];

      if (apiService.currentUser?.role == 'TAX_COLLECTOR') {
        // Feature: Auto-Populate PID from QR Code Scan to Tax Collector Dashboard
        if (targetPid != null && targetPid.isNotEmpty) {
          _searchController.text = targetPid;
          _searchPropertyDetails(apiService);
        } else if (token != null && token.isNotEmpty) {
          // If no PID in payload, fallback to looking up the token
          setState(() { _isSearching = true; });
          try {
            final card = await apiService.lookupCard(token);
            if (!mounted) return;
            if (card.propertyId.isNotEmpty) {
              _searchController.text = card.propertyId;
              _searchPropertyDetails(apiService);
            } else {
              setState(() { _isSearching = false; });
            }
          } catch (e) {
            if (!mounted) return;
            setState(() {
              _isSearching = false;
              _errorMessage = e.toString().replaceAll('Exception: ', '');
            });
            showDialog(
              context: context,
              builder: (ctx) => AlertDialog(
                title: const Text('Invalid Card'),
                content: Text(_errorMessage ?? 'Unknown error'),
                actions: [
                  TextButton(onPressed: () => Navigator.pop(ctx), child: const Text('OK')),
                ],
              ),
            );
          }
        }
      } else {
        // Regular FIELD_STAFF logic for unassigned cards
        if (targetPid != null && targetPid.isNotEmpty) {
          _searchController.text = targetPid;
          _searchPropertyDetails(apiService);
        } else if (token != null && token.isNotEmpty) {
          setState(() { _isSearching = true; });
          try {
            final card = await apiService.lookupCard(token);
            if (!mounted) return;
            if (card.propertyId.isNotEmpty) {
              _searchController.text = card.propertyId;
              _searchPropertyDetails(apiService);
            } else {
              setState(() { _isSearching = false; });
              Navigator.pushNamed(context, '/activate_form', arguments: card);
            }
          } catch (e) {
            if (!mounted) return;
            setState(() {
              _isSearching = false;
              _errorMessage = e.toString().replaceAll('Exception: ', '');
            });
            showDialog(
              context: context,
              builder: (ctx) => AlertDialog(
                title: const Text('Invalid Card'),
                content: Text(_errorMessage ?? 'Unknown error'),
                actions: [
                  TextButton(onPressed: () => Navigator.pop(ctx), child: const Text('OK')),
                ],
              ),
            );
          }
        }
      }
    });
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }
  @override
  Widget build(BuildContext context) {
    final apiService = Provider.of<ApiService>(context);
    final user = apiService.currentUser;
    final activations = apiService.recentActivations;

    return Scaffold(
      appBar: AppBar(
        title: Text(
          'PMC Smart QR Activation',
          style: GoogleFonts.outfit(
            fontWeight: FontWeight.bold,
            color: Colors.white,
            fontSize: 20,
          ),
        ),
        centerTitle: true,
        backgroundColor: PmcTheme.primaryBlue,
        iconTheme: const IconThemeData(color: Colors.white),
        actions: [
          IconButton(
            icon: const Icon(Icons.logout_rounded, color: Colors.white),
            tooltip: 'Logout',
            onPressed: () async {
              await apiService.logout();
              Navigator.pushNamedAndRemoveUntil(context, '/landing_scan', (route) => false);
            },
          ),
        ],
      ),
      drawer: _buildSettingsDrawer(context, apiService),
      body: SingleChildScrollView(
        child: Padding(
          padding: const EdgeInsets.all(20.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              // 1. Welcome Card
              Card(
                child: Container(
                  padding: const EdgeInsets.all(20),
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(20),
                    gradient: const LinearGradient(
                      colors: [PmcTheme.primaryBlue, Color(0xFF1E3E62)],
                      begin: Alignment.topLeft,
                      end: Alignment.bottomRight,
                    ),
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  'Welcome Back,',
                                  style: GoogleFonts.outfit(
                                    color: Colors.white.withOpacity(0.7),
                                    fontSize: 14,
                                  ),
                                ),
                                const SizedBox(height: 4),
                                Text(
                                  user?.fullName ?? 'Field Officer',
                                  style: GoogleFonts.outfit(
                                    color: Colors.white,
                                    fontSize: 22,
                                    fontWeight: FontWeight.bold,
                                  ),
                                  overflow: TextOverflow.ellipsis,
                                ),
                              ],
                            ),
                          ),
                          Container(
                            padding: const EdgeInsets.symmetric(
                              horizontal: 12,
                              vertical: 6,
                            ),
                            decoration: BoxDecoration(
                              color: PmcTheme.secondaryOrange,
                              borderRadius: BorderRadius.circular(30),
                            ),
                            child: Text(
                              user?.role ?? 'STAFF',
                              style: GoogleFonts.outfit(
                                color: Colors.white,
                                fontSize: 11,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 20),
                      const Divider(color: Colors.white24, height: 1),
                      const SizedBox(height: 16),
                      Row(
                        children: [
                          Expanded(
                            child: _buildInfoItem(
                              Icons.location_city_rounded,
                              'Circle',
                              user?.circle ?? 'Patliputra',
                            ),
                          ),
                          Expanded(
                            child: _buildInfoItem(
                              Icons.grid_3x3_rounded,
                              'Ward No.',
                              user?.wardNo?.toString() ?? '1',
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 24),

              // Tax Collector Property Search
              if (user?.role == 'TAX_COLLECTOR') ...[
                _buildTaxCollectorSearchSection(apiService),
                const SizedBox(height: 28),
              ],

              // 2. Statistics Row (Hidden for Tax Collectors)
              if (user?.role == 'FIELD_STAFF' || user?.role == 'ADMIN') ...[
                Row(
                  children: [
                    Expanded(
                      child: _buildStatCard(
                        context,
                        'Activations Today',
                        apiService.activationCount.toString(),
                        Icons.check_circle_rounded,
                        PmcTheme.successGreen,
                      ),
                    ),
                    const SizedBox(width: 16),
                    Expanded(
                      child: _buildStatCard(
                        context,
                        'Sync Queue',
                        '0 (Synced)',
                        Icons.sync_rounded,
                        PmcTheme.primaryBlue,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 28),
              ],

              // 3. Scan QR Trigger Button (Hidden for Tax Collectors)
              if (user?.role == 'FIELD_STAFF' || user?.role == 'ADMIN') ...[
                ElevatedButton(
                  onPressed: () {
                    Navigator.pushNamed(context, '/scan');
                  },
                  style: ElevatedButton.styleFrom(
                    backgroundColor: PmcTheme.secondaryOrange,
                    shadowColor: PmcTheme.secondaryOrange.withOpacity(0.4),
                    elevation: 6,
                    padding: const EdgeInsets.symmetric(vertical: 20),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(24),
                    ),
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const Icon(
                        Icons.qr_code_scanner_rounded,
                        size: 28,
                        color: Colors.white,
                      ),
                      const SizedBox(width: 12),
                      Text(
                        'ACTIVATE NEW CARD',
                        style: GoogleFonts.outfit(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                          letterSpacing: 1.0,
                          color: Colors.white,
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 32),
              ],

              // 4. Recent Activations Section (Hidden for Tax Collectors)
              if (user?.role == 'FIELD_STAFF' || user?.role == 'ADMIN') ...[
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'Recent Activations',
                      style: Theme.of(context).textTheme.titleMedium?.copyWith(
                        color: PmcTheme.primaryBlue,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    if (activations.isNotEmpty)
                      TextButton(
                        onPressed: () => apiService.clearHistory(),
                        child: Text(
                          'Clear History',
                          style: GoogleFonts.outfit(
                            color: PmcTheme.dangerRed,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ),
                  ],
                ),
                const SizedBox(height: 12),

                if (activations.isEmpty)
                  _buildEmptyHistoryCard()
                else
                  ListView.builder(
                    shrinkWrap: true,
                    physics: const NeverScrollableScrollPhysics(),
                    itemCount: activations.length,
                    itemBuilder: (context, index) {
                      final log = activations[index];
                      return _buildHistoryListItem(log);
                    },
                  ),
              ],
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildInfoItem(IconData icon, String label, String value) {
    return Row(
      children: [
        Icon(icon, color: Colors.white70, size: 20),
        const SizedBox(width: 8),
        Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              label,
              style: GoogleFonts.outfit(color: Colors.white60, fontSize: 11),
            ),
            Text(
              value,
              style: GoogleFonts.outfit(
                color: Colors.white,
                fontWeight: FontWeight.bold,
                fontSize: 14,
              ),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildStatCard(
    BuildContext context,
    String label,
    String value,
    IconData icon,
    Color color,
  ) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Icon(icon, color: color, size: 28),
            const SizedBox(height: 12),
            Text(
              value,
              style: GoogleFonts.outfit(
                fontSize: 20,
                fontWeight: FontWeight.bold,
                color: PmcTheme.textDark,
              ),
            ),
            const SizedBox(height: 4),
            Text(
              label,
              style: GoogleFonts.outfit(
                fontSize: 12,
                color: PmcTheme.textLight,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildEmptyHistoryCard() {
    return Card(
      elevation: 1,
      color: Colors.white,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(20),
        side: BorderSide(color: Colors.grey.shade100),
      ),
      child: Padding(
        padding: const EdgeInsets.symmetric(vertical: 36.0, horizontal: 16),
        child: Column(
          children: [
            Icon(Icons.history_rounded, size: 48, color: Colors.grey.shade300),
            const SizedBox(height: 12),
            Text(
              'No registrations in this session',
              style: GoogleFonts.outfit(
                fontSize: 16,
                fontWeight: FontWeight.w600,
                color: PmcTheme.textDark,
              ),
            ),
            const SizedBox(height: 4),
            Text(
              'Activated cards will appear here.',
              style: GoogleFonts.outfit(
                fontSize: 13,
                color: PmcTheme.textLight,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildHistoryListItem(Map<String, dynamic> log) {
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Row(
          children: [
            Container(
              padding: const EdgeInsets.all(10),
              decoration: BoxDecoration(
                color: PmcTheme.successGreen.withOpacity(0.1),
                shape: BoxShape.circle,
              ),
              child: const Icon(
                Icons.link_rounded,
                color: PmcTheme.successGreen,
                size: 24,
              ),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Property PID: ${log['pid']}',
                    style: GoogleFonts.outfit(
                      fontWeight: FontWeight.bold,
                      fontSize: 15,
                      color: PmcTheme.textDark,
                    ),
                  ),
                  const SizedBox(height: 2),
                  Text(
                    'Card: ${log['qrId']}',
                    style: GoogleFonts.outfit(
                      fontSize: 13,
                      color: PmcTheme.textLight,
                    ),
                  ),
                  if (log['owner'] != null) ...[
                    const SizedBox(height: 2),
                    Text(
                      log['owner'].toString(),
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
              children: [
                const Icon(
                  Icons.verified_rounded,
                  color: PmcTheme.successGreen,
                  size: 18,
                ),
                const SizedBox(height: 8),
                Text(
                  log['activatedAt'] != null
                      ? log['activatedAt'].toString().split(' ').last
                      : '',
                  style: GoogleFonts.outfit(
                    fontSize: 11,
                    color: PmcTheme.textLight,
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildSettingsDrawer(BuildContext context, ApiService apiService) {
    final urlController = TextEditingController(text: apiService.baseUrl);

    return Drawer(
      child: Column(
        children: [
          DrawerHeader(
            decoration: const BoxDecoration(color: PmcTheme.primaryBlue),
            child: Align(
              alignment: Alignment.bottomLeft,
              child: Column(
                mainAxisAlignment: MainAxisAlignment.end,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Icon(
                    Icons.settings_suggest_rounded,
                    color: Colors.white,
                    size: 36,
                  ),
                  const SizedBox(height: 8),
                  Text(
                    'System Configuration',
                    style: GoogleFonts.outfit(
                      color: Colors.white,
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Text(
                    'Developer & Supervisor Panel',
                    style: GoogleFonts.outfit(
                      color: Colors.white60,
                      fontSize: 11,
                    ),
                  ),
                ],
              ),
            ),
          ),
          Expanded(
            child: ListView(
              padding: const EdgeInsets.symmetric(horizontal: 16),
              children: [
                const SizedBox(height: 16),

                // Simulation Mode Switch


                // Resident Portal Simulator Option
                ListTile(
                  leading: const Icon(
                    Icons.public_rounded,
                    color: PmcTheme.secondaryOrange,
                  ),
                  title: Text(
                    'Resident Smart QR Portal',
                    style: GoogleFonts.outfit(
                      fontWeight: FontWeight.bold,
                      color: PmcTheme.textDark,
                    ),
                  ),
                  subtitle: Text(
                    'Simulate how citizens verify property dues after installation',
                    style: GoogleFonts.outfit(fontSize: 12),
                  ),
                  trailing: const Icon(
                    Icons.arrow_forward_ios_rounded,
                    size: 14,
                  ),
                  onTap: () {
                    Navigator.pop(context); // Close drawer
                    Navigator.pushNamed(context, '/citizen_portal');
                  },
                ),

                const Divider(),
                const SizedBox(height: 16),

                // Base URL Config
                Text(
                  'C# Backend Server API URL',
                  style: GoogleFonts.outfit(
                    fontWeight: FontWeight.bold,
                    color: PmcTheme.textDark,
                  ),
                ),
                const SizedBox(height: 8),
                TextField(
                  controller: urlController,
                  decoration: const InputDecoration(
                    hintText: 'e.g. http://187.127.178.111/api',
                    prefixIcon: Icon(Icons.link_rounded),
                  ),
                ),
                const SizedBox(height: 12),
                ElevatedButton(
                  onPressed: () {
                    apiService.setBaseUrl(urlController.text);
                    ScaffoldMessenger.of(context).showSnackBar(
                      const SnackBar(
                        content: Text('C# API base URL updated successfully!'),
                      ),
                    );
                    Navigator.pop(context); // Close drawer
                  },
                  child: const Text('SAVE ENDPOINT URL'),
                ),

                const SizedBox(height: 40),
                const Divider(),

                // Diagnostic Table List
                ListTile(
                  leading: const Icon(
                    Icons.storage_rounded,
                    color: PmcTheme.primaryBlue,
                  ),
                  title: Text(
                    'Table Details (Reference)',
                    style: GoogleFonts.outfit(
                      fontWeight: FontWeight.w600,
                      fontSize: 14,
                    ),
                  ),
                  subtitle: const Text(
                    'DB: pmc_sparrow_data\nTable: QRMaster\nAuth: tbl_QR_StaffUsers',
                  ),
                ),
              ],
            ),
          ),
          Padding(
            padding: const EdgeInsets.all(16.0),
            child: Text(
              'Patna Nagar Nigam v1.0.0',
              style: GoogleFonts.outfit(
                color: PmcTheme.textLight,
                fontSize: 11,
              ),
            ),
          ),
        ],
      ),
    );
  }

  Future<void> _searchPropertyDetails(ApiService apiService) async {
    final pid = _searchController.text.trim();
    if (pid.isEmpty) return;

    setState(() {
      _isSearching = true;
      _errorMessage = null;
      _searchResults = null;
      _taxHistoryResults = null;
    });

    try {
      final results = await apiService.lookupTaxCollectorPropertyDetails(pid);
      List<TaxHistoryRow>? taxHistory;
      try {
        taxHistory = await apiService.fetchTaxHistory(pid);
      } catch (e) {
        // Silently ignore if tax history not found, to keep showing property details
        print('Could not fetch tax history: $e');
      }
      setState(() {
        _isSearching = false;
        _searchResults = results;
        _taxHistoryResults = taxHistory;
      });
    } catch (e) {
      setState(() {
        _isSearching = false;
        _errorMessage = e.toString().replaceAll('Exception: ', '');
      });
    }
  }

  Widget _buildTaxCollectorSearchSection(ApiService apiService) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(
          'Tax Collector Dashboard',
          style: GoogleFonts.outfit(
            fontSize: 18,
            fontWeight: FontWeight.bold,
            color: PmcTheme.primaryBlue,
          ),
        ),
        const SizedBox(height: 12),
        Card(
          elevation: 2,
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
          child: Padding(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Row(
                  children: [
                    Expanded(
                      child: TextField(
                        controller: _searchController,
                        decoration: const InputDecoration(
                          labelText: 'Search Property ID (PID) or SAS No',
                          prefixIcon: Icon(Icons.search_rounded),
                        ),
                        onSubmitted: (_) => _searchPropertyDetails(apiService),
                      ),
                    ),
                    const SizedBox(width: 10),
                    ElevatedButton(
                      onPressed: _isSearching ? null : () => _searchPropertyDetails(apiService),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: PmcTheme.secondaryOrange,
                        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 16),
                      ),
                      child: _isSearching
                          ? const SizedBox(width: 20, height: 20, child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2))
                          : const Text('SEARCH'),
                    ),
                  ],
                ),
                if (_errorMessage != null) ...[
                  const SizedBox(height: 16),
                  Container(
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      color: PmcTheme.dangerRed.withOpacity(0.1),
                      borderRadius: BorderRadius.circular(8),
                      border: Border.all(color: PmcTheme.dangerRed.withOpacity(0.3)),
                    ),
                    child: Text(
                      _errorMessage!,
                      style: GoogleFonts.outfit(color: PmcTheme.dangerRed, fontSize: 13),
                    ),
                  ),
                ],
              ],
            ),
          ),
        ),
        if (_searchResults != null && _searchResults!.isNotEmpty) ...[
          const SizedBox(height: 20),
          // Owner Details Card (Common for all Occupancy rows)
          Card(
            margin: const EdgeInsets.only(bottom: 16),
            elevation: 3,
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
            color: PmcTheme.primaryBlue.withValues(alpha: 0.05),
            child: Padding(
              padding: const EdgeInsets.all(20.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Row(
                    children: [
                      const Icon(Icons.person_pin_rounded, color: PmcTheme.secondaryOrange),
                      const SizedBox(width: 8),
                      Expanded(
                        child: Text(
                          'Owner Details',
                          style: GoogleFonts.outfit(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                            color: PmcTheme.secondaryOrange,
                          ),
                        ),
                      ),
                      if (_taxHistoryResults != null && _taxHistoryResults!.isNotEmpty)
                        Container(
                          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
                          decoration: BoxDecoration(
                            color: PmcTheme.dangerRed,
                            borderRadius: BorderRadius.circular(12),
                          ),
                          child: Text(
                            '₹ ${_taxHistoryResults!.last.totalTax} (Total Tax)',
                            style: GoogleFonts.outfit(
                              color: Colors.white,
                              fontWeight: FontWeight.bold,
                              fontSize: 14,
                            ),
                          ),
                        ),
                    ],
                  ),
                  const SizedBox(height: 12),
                  const Divider(height: 1),
                  const SizedBox(height: 12),
                  _buildExtendedDetailRow('Property ID (PID)', _searchResults!.first.pid, isBold: true),
                  _buildExtendedDetailRow('SAS / Application No', _searchResults!.first.applicationNo, isBold: true),
                  _buildExtendedDetailRow('Owner Name', _searchResults!.first.ownerName),
                  _buildExtendedDetailRow('Guardian Name', _searchResults!.first.guardianName),
                  _buildExtendedDetailRow('Address', _searchResults!.first.address),
                  _buildExtendedDetailRow('Total Outstanding Dues', '₹ ${_searchResults!.first.totalDues}', isBold: true),
                ],
              ),
            ),
          ),
          
          if (_taxHistoryResults != null && _taxHistoryResults!.isNotEmpty)
            Card(
              margin: const EdgeInsets.only(bottom: 16),
              elevation: 3,
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
              child: Padding(
                padding: const EdgeInsets.all(20.0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    Row(
                      children: [
                        const Icon(Icons.history_rounded, color: PmcTheme.primaryBlue),
                        const SizedBox(width: 8),
                        Expanded(
                          child: Text(
                            'Property Tax History',
                            style: GoogleFonts.outfit(
                              fontSize: 16,
                              fontWeight: FontWeight.bold,
                              color: PmcTheme.primaryBlue,
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 12),
                    Table(
                      border: TableBorder(
                        horizontalInside: BorderSide(color: Colors.grey.shade200),
                        bottom: BorderSide(color: Colors.grey.shade200),
                      ),
                      columnWidths: const {
                        0: FlexColumnWidth(1),
                        1: FlexColumnWidth(1.2),
                        2: FlexColumnWidth(1),
                        3: FlexColumnWidth(1),
                        4: FlexColumnWidth(1.2),
                      },
                      children: [
                        TableRow(
                          decoration: BoxDecoration(color: PmcTheme.primaryBlue.withOpacity(0.05)),
                          children: const [
                            Padding(padding: EdgeInsets.symmetric(vertical: 12, horizontal: 8), child: Text('Fin Year', style: TextStyle(fontWeight: FontWeight.bold))),
                            Padding(padding: EdgeInsets.symmetric(vertical: 12, horizontal: 8), child: Text('Property ID', style: TextStyle(fontWeight: FontWeight.bold))),
                            Padding(padding: EdgeInsets.symmetric(vertical: 12, horizontal: 8), child: Text('Floor Tax', style: TextStyle(fontWeight: FontWeight.bold))),
                            Padding(padding: EdgeInsets.symmetric(vertical: 12, horizontal: 8), child: Text('Vacant Tax', style: TextStyle(fontWeight: FontWeight.bold))),
                            Padding(padding: EdgeInsets.symmetric(vertical: 12, horizontal: 8), child: Text('Total Tax', style: TextStyle(fontWeight: FontWeight.bold))),
                          ],
                        ),
                        ..._taxHistoryResults!.map((row) {
                          final isTotal = row.finYear.toUpperCase() == 'TOTAL';
                          return TableRow(
                            decoration: isTotal ? BoxDecoration(color: PmcTheme.secondaryOrange.withOpacity(0.1)) : null,
                            children: [
                              Padding(padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 8), child: Text(row.finYear, style: TextStyle(fontWeight: isTotal ? FontWeight.bold : FontWeight.normal))),
                              Padding(padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 8), child: Text(row.propertyId)),
                              Padding(padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 8), child: Text(row.floorTax)),
                              Padding(padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 8), child: Text(row.vacantTax)),
                              Padding(padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 8), child: Text(row.totalTax, style: TextStyle(fontWeight: isTotal ? FontWeight.bold : FontWeight.normal))),
                            ],
                          );
                        }).toList(),
                      ],
                    ),
                  ],
                ),
              ),
            ),
          
          ..._searchResults!.asMap().entries.map((entry) {
            final detail = entry.value;
            final index = entry.key;
            return Card(
              margin: const EdgeInsets.only(bottom: 16),
              elevation: 2,
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
              child: Padding(
                padding: const EdgeInsets.all(20.0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    Row(
                      children: [
                        const Icon(Icons.maps_home_work_rounded, color: PmcTheme.primaryBlue),
                        const SizedBox(width: 8),
                        Expanded(
                          child: Text(
                            'Occupancy Details ${_searchResults!.length > 1 ? "(#${index + 1})" : ""}',
                            style: GoogleFonts.outfit(
                              fontSize: 16,
                              fontWeight: FontWeight.bold,
                              color: PmcTheme.primaryBlue,
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 12),
                    const Divider(height: 1),
                    const SizedBox(height: 12),
                    _buildExtendedDetailRow('Assessment Year', detail.assessmentYear),
                    _buildExtendedDetailRow('Plot Area', '${detail.plotArea} sq.ft'),
                    _buildExtendedDetailRow('Constructed Area', '${detail.constructedArea} sq.ft'),
                    _buildExtendedDetailRow('Builtup Area', '${detail.builtupArea} sq.ft'),
                    _buildExtendedDetailRow('Street Type', detail.streetType),
                    _buildExtendedDetailRow('Floor No', detail.floorNo),
                    _buildExtendedDetailRow('Use Type', detail.useType),
                    _buildExtendedDetailRow('Usage Type', detail.usageType),
                    _buildExtendedDetailRow('Construction Type', detail.contructionType),
                    _buildExtendedDetailRow('Occupancy Type', detail.occupancyType),
                  ],
                ),
              ),
            );
          }).toList(),
        ],
      ],
    );
  }

  Widget _buildExtendedDetailRow(String label, String value, {bool isBold = false}) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4.0),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Expanded(
            flex: 2,
            child: Text(
              label,
              style: GoogleFonts.outfit(color: PmcTheme.textLight, fontSize: 13),
            ),
          ),
          Expanded(
            flex: 3,
            child: Text(
              value.isEmpty ? 'N/A' : value,
              textAlign: TextAlign.end,
              style: GoogleFonts.outfit(
                fontWeight: isBold ? FontWeight.bold : FontWeight.w600,
                fontSize: 13,
                color: PmcTheme.textDark,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

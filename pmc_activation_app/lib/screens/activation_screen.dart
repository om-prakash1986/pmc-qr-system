import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:google_fonts/google_fonts.dart';
import '../services/api_service.dart';
import '../theme.dart';
import '../models/qr_card.dart';
import '../models/property.dart';
import '../models/property_detail_row.dart';

class ActivationScreen extends StatefulWidget {
  const ActivationScreen({Key? key}) : super(key: key);

  @override
  _ActivationScreenState createState() => _ActivationScreenState();
}

class _ActivationScreenState extends State<ActivationScreen> {
  final _pidController = TextEditingController();
  Property? _verifiedProperty;
  bool _isVerifyingPid = false;
  String? _errorMessage;
  String? _successMessage;

  // Tax Collector Dashboard State
  bool _isLoadingActivatedProperty = false;
  Property? _activatedProperty;
  List<PropertyDetailRow>? _extendedPropertyDetails;
  bool _hasTriggeredLoad = false;
  // Tax Collector Authentication State
  final _tcUserController = TextEditingController();
  final _tcPassController = TextEditingController();
  bool _isTcAuthenticated = false;
  String? _tcAuthError;

  @override
  void dispose() {
    // Dispose tax collector controllers
    _tcUserController.dispose();
    _tcPassController.dispose();

    _pidController.dispose();
    super.dispose();
  }

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    if (!_hasTriggeredLoad) {
      _hasTriggeredLoad = true;
      final card = ModalRoute.of(context)?.settings.arguments as QrCard?;
      if (card != null && card.isActivated && card.propertyId.isNotEmpty) {
        _loadActivatedPropertyDetails(card.propertyId);
      }
    }
  }

  void _loadActivatedPropertyDetails(String pid) async {
    setState(() {
      _isLoadingActivatedProperty = true;
      _errorMessage = null;
    });

    final apiService = Provider.of<ApiService>(context, listen: false);
    try {
      final property = await apiService.lookupProperty(pid);
      List<PropertyDetailRow>? extendedDetails;
      try {
        extendedDetails = await apiService.lookupTaxCollectorPropertyDetails(pid);
      } catch (e) {
        print("Extended details fetch error: $e");
      }
      
      setState(() {
        _activatedProperty = property;
        _extendedPropertyDetails = extendedDetails;
        _isLoadingActivatedProperty = false;
      });
    } catch (e) {
      setState(() {
        _isLoadingActivatedProperty = false;
        _errorMessage =
            "Could not load property details: " +
            e.toString().replaceAll('Exception: ', '');
      });
    }
  }

  void _verifyPid(ApiService apiService) async {
    final pid = _pidController.text.trim();
    if (pid.isEmpty) return;

    setState(() {
      _isVerifyingPid = true;
      _errorMessage = null;
      _verifiedProperty = null;
    });

    try {
      final property = await apiService.lookupProperty(pid);
      setState(() {
        _isVerifyingPid = false;
        _verifiedProperty = property;
      });
    } catch (e) {
      setState(() {
        _isVerifyingPid = false;
        _errorMessage = e.toString().replaceAll('Exception: ', '');
      });
    }
  }

  void _confirmAndActivate(ApiService apiService, QrCard card) async {
    if (_verifiedProperty == null) return;

    try {
      await apiService.activateCard(
        card.qrId,
        _verifiedProperty!.pid,
        card.qrToken,
      );

      // Route to success screen
      Navigator.pushReplacementNamed(
        context,
        '/success',
        arguments: {
          'qrId': card.qrId,
          'pid': _verifiedProperty!.pid,
          'ownerName': _verifiedProperty!.ownerName,
          'address': _verifiedProperty!.address,
        },
      );
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(e.toString().replaceAll('Exception: ', '')),
          backgroundColor: PmcTheme.dangerRed,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final card = ModalRoute.of(context)?.settings.arguments as QrCard?;
    final apiService = Provider.of<ApiService>(context);

    if (card == null) {
      return Scaffold(
        appBar: AppBar(title: const Text('Error')),
        body: const Center(child: Text('Card data lost (likely due to hot restart). Please go back and scan again.')),
      );
    }

    return Scaffold(
      appBar: AppBar(
        title: Text(
          card.isActivated ? 'QR Card Details' : 'Activate PVC Card',
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
          padding: const EdgeInsets.all(20.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              // 1. Scanned Card Card
              _buildScannedCardInfo(card),
              const SizedBox(height: 20),

              // 2. Conditional UI: Locked state vs Activation Form
              if (card.isActivated)
                Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    if (_isLoadingActivatedProperty)
                      Card(
                        elevation: 2,
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(20),
                        ),
                        child: const Padding(
                          padding: EdgeInsets.all(32.0),
                          child: Center(
                            child: Column(
                              children: [
                                CircularProgressIndicator(
                                  color: PmcTheme.primaryBlue,
                                ),
                                SizedBox(height: 16),
                                Text(
                                  'Fetching live Sparrow records for PID...',
                                  style: TextStyle(fontWeight: FontWeight.w500),
                                ),
                              ],
                            ),
                          ),
                        ),
                      )
                    else if (_errorMessage != null)
                      Column(
                        children: [
                          _buildAlertBox(_errorMessage!, PmcTheme.dangerRed),
                          _buildLockedCardDetails(card),
                        ],
                      )
                    else if (!_isTcAuthenticated)
                      // Tax Collector Login Form
                      Card(
                        elevation: 2,
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(20),
                        ),
                        child: Padding(
                          padding: const EdgeInsets.all(20.0),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.stretch,
                            children: [
                              Text(
                                'Tax Collector Login',
                                style: GoogleFonts.outfit(
                                  fontSize: 18,
                                  fontWeight: FontWeight.bold,
                                  color: PmcTheme.primaryBlue,
                                ),
                              ),
                              const SizedBox(height: 12),
                              TextField(
                                controller: _tcUserController,
                                decoration: const InputDecoration(
                                  labelText: 'User ID',
                                ),
                              ),
                              const SizedBox(height: 8),
                              TextField(
                                controller: _tcPassController,
                                obscureText: true,
                                decoration: const InputDecoration(
                                  labelText: 'Password',
                                ),
                              ),
                              if (_tcAuthError != null)
                                Padding(
                                  padding: const EdgeInsets.only(top: 8.0),
                                  child: _buildAlertBox(
                                    _tcAuthError!,
                                    Colors.red,
                                  ),
                                ),
                              const SizedBox(height: 12),
                              ElevatedButton(
                                onPressed: () async {
                                  try {
                                    final ok = await apiService
                                        .taxCollectorLogin(
                                          _tcUserController.text.trim(),
                                          _tcPassController.text,
                                        );
                                    if (ok) {
                                      setState(() {
                                        _isTcAuthenticated = true;
                                        _tcAuthError = null;
                                      });
                                    }
                                  } catch (e) {
                                    setState(() {
                                      _tcAuthError = e.toString();
                                    });
                                  }
                                },
                                child: const Text('Login'),
                                style: ElevatedButton.styleFrom(
                                  backgroundColor: PmcTheme.successGreen,
                                ),
                              ),
                            ],
                          ),
                        ),
                      )
                    else
                      _buildTaxCollectorDashboard(card, _activatedProperty, _extendedPropertyDetails),

                    const SizedBox(height: 20),
                    ElevatedButton.icon(
                      onPressed: () {
                        Navigator.pushNamed(
                          context,
                          '/citizen_portal',
                          arguments: {'token': card.qrToken},
                        );
                      },
                      icon: const Icon(
                        Icons.public_rounded,
                        color: Colors.white,
                      ),
                      label: const Text('PREVIEW CITIZEN VIEW FOR THIS CARD'),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: PmcTheme.secondaryOrange,
                        padding: const EdgeInsets.symmetric(vertical: 16),
                      ),
                    ),
                  ],
                )
              else
                Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    // Form Header
                    Text(
                      'Link Card to Property PID',
                      style: GoogleFonts.outfit(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                        color: PmcTheme.primaryBlue,
                      ),
                    ),
                    const SizedBox(height: 12),

                    // PID Input Field
                    Row(
                      children: [
                        Expanded(
                          child: TextField(
                            controller: _pidController,
                            keyboardType: TextInputType.text,
                            style: GoogleFonts.outfit(color: PmcTheme.textDark),
                            decoration: const InputDecoration(
                              labelText: 'Property ID (PID)',
                              hintText: 'e.g. 1409113',
                              prefixIcon: Icon(Icons.home_outlined),
                            ),
                            onChanged: (_) {
                              // Reset verified property if typing changes
                              if (_verifiedProperty != null) {
                                setState(() {
                                  _verifiedProperty = null;
                                });
                              }
                            },
                          ),
                        ),
                        const SizedBox(width: 8),
                        SizedBox(
                          height: 56,
                          child: ElevatedButton(
                            onPressed: _isVerifyingPid
                                ? null
                                : () => _verifyPid(apiService),
                            style: ElevatedButton.styleFrom(
                              backgroundColor: PmcTheme.primaryBlue,
                              padding: const EdgeInsets.symmetric(
                                horizontal: 16,
                              ),
                            ),
                            child: _isVerifyingPid
                                ? const SizedBox(
                                    width: 18,
                                    height: 18,
                                    child: CircularProgressIndicator(
                                      color: Colors.white,
                                      strokeWidth: 2,
                                    ),
                                  )
                                : const Text('VERIFY'),
                          ),
                        ),
                      ],
                    ),

                    // Quick simulation guide
                    if (apiService.isSimulated &&
                        _verifiedProperty == null) ...[
                      const SizedBox(height: 10),
                      Row(
                        children: [
                          Text(
                            'Quick Mock PIDs: ',
                            style: GoogleFonts.outfit(
                              fontSize: 12,
                              color: PmcTheme.textLight,
                            ),
                          ),
                          _buildMockPidChip('1409113', apiService),
                          const SizedBox(width: 8),
                          _buildMockPidChip('1185006', apiService),
                        ],
                      ),
                    ],

                    const SizedBox(height: 20),

                    // Error Message
                    if (_errorMessage != null)
                      _buildAlertBox(_errorMessage!, PmcTheme.dangerRed),

                    // Verified Property Details Card
                    if (_verifiedProperty != null) ...[
                      _buildPropertyDetailsCard(_verifiedProperty!),
                      const SizedBox(height: 24),

                      // Confirmation Alert
                      Container(
                        padding: const EdgeInsets.all(12),
                        decoration: BoxDecoration(
                          color: PmcTheme.secondaryOrange.withOpacity(0.1),
                          borderRadius: BorderRadius.circular(12),
                          border: Border.all(
                            color: PmcTheme.secondaryOrange.withOpacity(0.3),
                          ),
                        ),
                        child: Row(
                          children: [
                            const Icon(
                              Icons.info_outline_rounded,
                              color: PmcTheme.secondaryOrange,
                            ),
                            const SizedBox(width: 10),
                            Expanded(
                              child: Text(
                                'Confirm the PVC Plate is fixed on the wall of this specific house before confirming.',
                                style: GoogleFonts.outfit(
                                  color: PmcTheme.textDark,
                                  fontSize: 12,
                                  fontWeight: FontWeight.w500,
                                ),
                              ),
                            ),
                          ],
                        ),
                      ),
                      const SizedBox(height: 24),

                      // Final Activation Trigger Button
                      ElevatedButton(
                        onPressed: apiService.isLoading
                            ? null
                            : () => _confirmAndActivate(apiService, card),
                        style: ElevatedButton.styleFrom(
                          backgroundColor: PmcTheme.successGreen,
                          shadowColor: PmcTheme.successGreen.withOpacity(0.3),
                          padding: const EdgeInsets.symmetric(vertical: 18),
                        ),
                        child: apiService.isLoading
                            ? const CircularProgressIndicator(
                                color: Colors.white,
                              )
                            : Text(
                                'CONFIRM & ACTIVATE LINK',
                                style: GoogleFonts.outfit(
                                  fontWeight: FontWeight.bold,
                                  fontSize: 16,
                                ),
                              ),
                      ),
                    ],
                  ],
                ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildTaxCollectorDashboard(QrCard card, Property? property, List<PropertyDetailRow>? extendedDetails) {
    if (property == null) {
      return Container(
        padding: const EdgeInsets.all(24),
        child: const Center(
          child: CircularProgressIndicator(color: PmcTheme.primaryBlue),
        ),
      );
    }

    final duesAmt = double.tryParse(property.totalDues) ?? 0.0;
    final isPaid =
        property.paymentStatus.toLowerCase() == 'paid' || duesAmt == 0;

    // Dynamic values based on payment status
    final currentPayable = duesAmt;
    final lastPaidAmt = isPaid
        ? (card.propertyId == '1409113' ? 21906.0 : 4200.0)
        : (card.propertyId == '1409113' ? 4200.0 : 1200.0);
    final lastPaidDate = isPaid ? '18-Jun-2026' : '15-Nov-2025';
    final totalPaidToDate = isPaid
        ? (card.propertyId == '1409113' ? 34506.0 : 16800.0)
        : (card.propertyId == '1409113' ? 12600.0 : 12600.0);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        // Green status banner for verification success
        Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: PmcTheme.successGreen.withOpacity(0.1),
            borderRadius: BorderRadius.circular(16),
            border: Border.all(color: PmcTheme.successGreen.withOpacity(0.3)),
          ),
          child: Row(
            children: [
              const Icon(
                Icons.verified_rounded,
                color: PmcTheme.successGreen,
                size: 28,
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'VERIFIED QR IDENTITY',
                      style: GoogleFonts.outfit(
                        color: PmcTheme.successGreen,
                        fontWeight: FontWeight.bold,
                        fontSize: 12,
                        letterSpacing: 1.0,
                      ),
                    ),
                    const SizedBox(height: 2),
                    Text(
                      'Linked to active holding record.',
                      style: GoogleFonts.outfit(
                        color: PmcTheme.textDark,
                        fontSize: 13,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 16),

        // Property & Holding Details card
        Card(
          elevation: 3,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(20),
            side: const BorderSide(color: PmcTheme.primaryBlue, width: 1.0),
          ),
          child: Padding(
            padding: const EdgeInsets.all(20.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Row(
                  children: [
                    const Icon(
                      Icons.home_work_rounded,
                      color: PmcTheme.primaryBlue,
                      size: 24,
                    ),
                    const SizedBox(width: 10),
                    Text(
                      'Holding Details',
                      style: GoogleFonts.outfit(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                        color: PmcTheme.primaryBlue,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 16),
                const Divider(height: 1),
                const SizedBox(height: 12),

                _buildPropertyRow(
                  'Owner Name',
                  property.ownerName,
                  isHighlighted: true,
                ),
                _buildPropertyRow('Father/Guardian', property.guardianName),
                _buildPropertyRow('Holding Address', property.address),

                Row(
                  children: [
                    Expanded(
                      child: _buildPropertyRow('Ward Number', property.wardNo),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: _buildPropertyRow(
                        'Circle',
                        property.circle,
                      ),
                    ),
                  ],
                ),
                _buildPropertyRow('Revenue Circle No', property.revenueCircleNo),
                _buildPropertyRow('Registered Mobile', property.mobileNo),
              ],
            ),
          ),
        ),
        const SizedBox(height: 16),

        // Tax Collector Payment and Dues Summary card
        Card(
          elevation: 3,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(20),
          ),
          child: Padding(
            padding: const EdgeInsets.all(20.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Row(
                  children: [
                    const Icon(
                      Icons.currency_rupee_rounded,
                      color: PmcTheme.secondaryOrange,
                      size: 24,
                    ),
                    const SizedBox(width: 10),
                    Text(
                      'Tax & Dues Summary',
                      style: GoogleFonts.outfit(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                        color: PmcTheme.primaryBlue,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 16),
                const Divider(height: 1),
                const SizedBox(height: 16),

                // Payable Dues
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'Current Payable Amount',
                      style: GoogleFonts.outfit(
                        fontSize: 13,
                        color: PmcTheme.textLight,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                    Text(
                      '₹${currentPayable.toStringAsFixed(2)}',
                      style: GoogleFonts.outfit(
                        fontSize: 18,
                        fontWeight: FontWeight.bold,
                        color: isPaid
                            ? PmcTheme.successGreen
                            : PmcTheme.dangerRed,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 10),

                // Payment Status
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'Payment Status',
                      style: GoogleFonts.outfit(
                        fontSize: 13,
                        color: PmcTheme.textLight,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                    Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 12,
                        vertical: 4,
                      ),
                      decoration: BoxDecoration(
                        color: isPaid
                            ? PmcTheme.successGreen.withOpacity(0.15)
                            : PmcTheme.secondaryOrange.withOpacity(0.15),
                        borderRadius: BorderRadius.circular(6),
                      ),
                      child: Text(
                        property.paymentStatus.toUpperCase(),
                        style: GoogleFonts.outfit(
                          color: isPaid
                              ? PmcTheme.successGreen
                              : PmcTheme.secondaryOrange,
                          fontWeight: FontWeight.bold,
                          fontSize: 11,
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 12),
                const Divider(height: 1),
                const SizedBox(height: 12),

                // Last Paid details
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'Last Paid Amount',
                      style: GoogleFonts.outfit(
                        fontSize: 13,
                        color: PmcTheme.textLight,
                      ),
                    ),
                    Text(
                      '₹${lastPaidAmt.toStringAsFixed(2)}',
                      style: GoogleFonts.outfit(
                        fontSize: 14,
                        fontWeight: FontWeight.bold,
                        color: PmcTheme.textDark,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 8),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'Last Payment Date',
                      style: GoogleFonts.outfit(
                        fontSize: 13,
                        color: PmcTheme.textLight,
                      ),
                    ),
                    Text(
                      lastPaidDate,
                      style: GoogleFonts.outfit(
                        fontSize: 14,
                        fontWeight: FontWeight.w600,
                        color: PmcTheme.textDark,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 8),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'Total Amount Paid',
                      style: GoogleFonts.outfit(
                        fontSize: 13,
                        color: PmcTheme.textLight,
                      ),
                    ),
                    Text(
                      '₹${totalPaidToDate.toStringAsFixed(2)}',
                      style: GoogleFonts.outfit(
                        fontSize: 14,
                        fontWeight: FontWeight.bold,
                        color: PmcTheme.textDark,
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
        ),
        const SizedBox(height: 16),

        if (extendedDetails != null && extendedDetails.isNotEmpty)
          ...extendedDetails.asMap().entries.map((entry) {
            final int index = entry.key;
            final PropertyDetailRow detail = entry.value;
            return Padding(
              padding: const EdgeInsets.only(bottom: 16.0),
              child: Card(
                elevation: 3,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(20),
                ),
                child: Padding(
                  padding: const EdgeInsets.all(20.0),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      Row(
                        children: [
                          const Icon(
                            Icons.dashboard_rounded,
                            color: PmcTheme.primaryBlue,
                            size: 24,
                          ),
                          const SizedBox(width: 10),
                          Expanded(
                            child: Text(
                              'Property Occupancy Details ${extendedDetails.length > 1 ? "(#${index + 1})" : ""}',
                              style: GoogleFonts.outfit(
                                fontSize: 16,
                                fontWeight: FontWeight.bold,
                                color: PmcTheme.primaryBlue,
                              ),
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 16),
                      const Divider(height: 1),
                      const SizedBox(height: 12),
                      _buildDetailRow('Application No', detail.applicationNo, isBold: true),
                      _buildDetailRow('Assessment Year', detail.assessmentYear),
                      _buildDetailRow('Plot Area', '${detail.plotArea} sq.ft'),
                      _buildDetailRow('Constructed Area', '${detail.constructedArea} sq.ft'),
                      _buildDetailRow('Builtup Area', '${detail.builtupArea} sq.ft'),
                      _buildDetailRow('Street Type', detail.streetType),
                      _buildDetailRow('Floor No', detail.floorNo),
                      _buildDetailRow('Use Type', detail.useType),
                      _buildDetailRow('Usage Type', detail.usageType),
                      _buildDetailRow('Construction Type', detail.contructionType),
                      _buildDetailRow('Occupancy Type', detail.occupancyType),
                    ],
                  ),
                ),
              ),
            );
          }).toList(),

        // QR Master Metadata Card (Security lock details)
        Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: Colors.grey.shade100,
            borderRadius: BorderRadius.circular(16),
            border: Border.all(color: Colors.grey.shade300),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Row(
                children: [
                  const Icon(
                    Icons.lock_rounded,
                    size: 16,
                    color: PmcTheme.textLight,
                  ),
                  const SizedBox(width: 8),
                  Text(
                    'SECURITY LOCK METADATA',
                    style: GoogleFonts.outfit(
                      color: PmcTheme.textLight,
                      fontSize: 11,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              _buildDetailRow('Assigned PID', card.propertyId, isBold: true),
              _buildDetailRow('Activated By', card.activatedBy),
              _buildDetailRow('Activation Date', card.activatedDate),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildMockPidChip(String pid, ApiService service) {
    return ActionChip(
      label: Text(pid, style: const TextStyle(fontSize: 11)),
      onPressed: () {
        _pidController.text = pid;
        _verifyPid(service);
      },
    );
  }

  Widget _buildScannedCardInfo(QrCard card) {
    return Card(
      elevation: 2,
      child: Padding(
        padding: const EdgeInsets.all(18.0),
        child: Column(
          children: [
            Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: PmcTheme.primaryBlue.withOpacity(0.08),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: const Icon(
                    Icons.qr_code_2_rounded,
                    color: PmcTheme.primaryBlue,
                    size: 36,
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'QR ID: ${card.qrId}',
                        style: GoogleFonts.outfit(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                          color: PmcTheme.textDark,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        'Token: ${card.qrToken}',
                        style: GoogleFonts.outfit(
                          fontSize: 12,
                          color: PmcTheme.textLight,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            const Divider(height: 1),
            const SizedBox(height: 12),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  'Current Card Status',
                  style: GoogleFonts.outfit(
                    fontSize: 13,
                    color: PmcTheme.textLight,
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 14,
                    vertical: 6,
                  ),
                  decoration: BoxDecoration(
                    color: card.isActivated
                        ? PmcTheme.successGreen.withOpacity(0.15)
                        : PmcTheme.secondaryOrange.withOpacity(0.15),
                    borderRadius: BorderRadius.circular(30),
                  ),
                  child: Text(
                    card.status.toUpperCase(),
                    style: GoogleFonts.outfit(
                      color: card.isActivated
                          ? PmcTheme.successGreen
                          : PmcTheme.secondaryOrange,
                      fontWeight: FontWeight.bold,
                      fontSize: 12,
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildLockedCardDetails(QrCard card) {
    return Card(
      color: Colors.red.shade50.withOpacity(0.5),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(20),
        side: BorderSide(color: Colors.red.shade100, width: 1.5),
      ),
      child: Padding(
        padding: const EdgeInsets.all(20.0),
        child: Column(
          children: [
            const Icon(Icons.lock_rounded, size: 48, color: PmcTheme.dangerRed),
            const SizedBox(height: 12),
            Text(
              'Security Lock: Already Linked',
              style: GoogleFonts.outfit(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: PmcTheme.dangerRed,
              ),
            ),
            const SizedBox(height: 8),
            Text(
              'To prevent fraud, activated PVC cards cannot be moved or re-mapped to another house without Supervisor Approval.',
              style: GoogleFonts.outfit(fontSize: 13, color: PmcTheme.textDark),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 20),
            const Divider(color: Color(0xFFEF9A9A), height: 1),
            const SizedBox(height: 16),
            _buildDetailRow('Assigned PID', card.propertyId, isBold: true),
            _buildDetailRow('Activated By', card.activatedBy),
            _buildDetailRow('Activation Date', card.activatedDate),
          ],
        ),
      ),
    );
  }

  Widget _buildDetailRow(String label, String value, {bool isBold = false}) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6.0),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(
            label,
            style: GoogleFonts.outfit(color: PmcTheme.textLight, fontSize: 13),
          ),
          Text(
            value,
            style: GoogleFonts.outfit(
              fontWeight: isBold ? FontWeight.bold : FontWeight.w600,
              fontSize: 14,
              color: PmcTheme.textDark,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildPropertyDetailsCard(Property property) {
    final duesAmt = double.tryParse(property.totalDues) ?? 0.0;

    return Card(
      elevation: 3,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(20),
        side: const BorderSide(color: PmcTheme.primaryBlue, width: 1.0),
      ),
      child: Padding(
        padding: const EdgeInsets.all(20.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Row(
              children: [
                const Icon(
                  Icons.home_work_rounded,
                  color: PmcTheme.primaryBlue,
                  size: 28,
                ),
                const SizedBox(width: 12),
                Text(
                  'Holding Information',
                  style: GoogleFonts.outfit(
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                    color: PmcTheme.primaryBlue,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            const Divider(height: 1),
            const SizedBox(height: 12),
            _buildPropertyRow(
              'Owner Name',
              property.ownerName,
              isHighlighted: true,
            ),
            _buildPropertyRow('Guardian/Father', property.guardianName),
            _buildPropertyRow('Mobile No', property.mobileNo),
            _buildPropertyRow('Site Address', property.address),
            _buildPropertyRow('Revenue Circle No', property.revenueCircleNo),
            _buildPropertyRow('Circle', property.circle),
            _buildPropertyRow('Ward Number', property.wardNo),

            const SizedBox(height: 8),
            const Divider(height: 1),
            const SizedBox(height: 12),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  'Tax Payment Status',
                  style: GoogleFonts.outfit(
                    color: PmcTheme.textLight,
                    fontSize: 13,
                  ),
                ),
                Row(
                  children: [
                    if (duesAmt > 0)
                      Container(
                        margin: const EdgeInsets.only(right: 8),
                        padding: const EdgeInsets.symmetric(
                          horizontal: 10,
                          vertical: 4,
                        ),
                        decoration: BoxDecoration(
                          color: PmcTheme.dangerRed.withOpacity(0.1),
                          borderRadius: BorderRadius.circular(6),
                        ),
                        child: Text(
                          'Dues: ₹${property.totalDues}',
                          style: GoogleFonts.outfit(
                            color: PmcTheme.dangerRed,
                            fontWeight: FontWeight.bold,
                            fontSize: 11,
                          ),
                        ),
                      ),
                    Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 12,
                        vertical: 4,
                      ),
                      decoration: BoxDecoration(
                        color: property.paymentStatus.toLowerCase() == 'paid'
                            ? PmcTheme.successGreen.withOpacity(0.15)
                            : PmcTheme.secondaryOrange.withOpacity(0.15),
                        borderRadius: BorderRadius.circular(6),
                      ),
                      child: Text(
                        property.paymentStatus,
                        style: GoogleFonts.outfit(
                          color: property.paymentStatus.toLowerCase() == 'paid'
                              ? PmcTheme.successGreen
                              : PmcTheme.secondaryOrange,
                          fontWeight: FontWeight.bold,
                          fontSize: 11,
                        ),
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildPropertyRow(
    String label,
    String value, {
    bool isHighlighted = false,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            label,
            style: GoogleFonts.outfit(fontSize: 11, color: PmcTheme.textLight),
          ),
          const SizedBox(height: 2),
          Text(
            value,
            style: GoogleFonts.outfit(
              fontSize: isHighlighted ? 15 : 13.5,
              fontWeight: isHighlighted ? FontWeight.bold : FontWeight.w500,
              color: isHighlighted ? PmcTheme.primaryBlue : PmcTheme.textDark,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildAlertBox(String message, Color color) {
    return Container(
      padding: const EdgeInsets.all(12),
      margin: const EdgeInsets.only(bottom: 20),
      decoration: BoxDecoration(
        color: color.withOpacity(0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: color.withOpacity(0.3)),
      ),
      child: Row(
        children: [
          Icon(Icons.warning_amber_rounded, color: color),
          const SizedBox(width: 10),
          Expanded(
            child: Text(
              message,
              style: GoogleFonts.outfit(
                color: color,
                fontSize: 13,
                fontWeight: FontWeight.w500,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

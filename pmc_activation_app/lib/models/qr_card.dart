class QrCard {
  final String qrId;
  final String qrToken;
  final String status;
  final String propertyId;
  final String createdDate;
  final String activatedDate;
  final String activatedBy;
  final String qrUrl;

  QrCard({
    required this.qrId,
    required this.qrToken,
    required this.status,
    required this.propertyId,
    required this.createdDate,
    required this.activatedDate,
    required this.activatedBy,
    required this.qrUrl,
  });

  bool get isUnassigned => status.toUpperCase() == 'UNASSIGNED';
  bool get isActivated => status.toUpperCase() == 'ACTIVATED';

  factory QrCard.fromJson(Map<String, dynamic> json) {
    return QrCard(
      qrId: json['qrId'] as String? ?? '',
      qrToken: json['qrToken'] as String? ?? '',
      status: json['status'] as String? ?? 'UNASSIGNED',
      propertyId: json['propertyId'] as String? ?? '',
      createdDate: json['createdDate'] as String? ?? '',
      activatedDate: json['activatedDate'] as String? ?? '',
      activatedBy: json['activatedBy'] as String? ?? '',
      qrUrl: json['qrUrl'] as String? ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'qrId': qrId,
      'qrToken': qrToken,
      'status': status,
      'propertyId': propertyId,
      'createdDate': createdDate,
      'activatedDate': activatedDate,
      'activatedBy': activatedBy,
      'qrUrl': qrUrl,
    };
  }
}

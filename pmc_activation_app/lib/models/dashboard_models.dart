class ActivationStats {
  final int totalActivations;
  final int successful;
  final int failed;
  final int pendingPayments;

  ActivationStats({
    required this.totalActivations,
    required this.successful,
    required this.failed,
    required this.pendingPayments,
  });

  factory ActivationStats.fromJson(Map<String, dynamic> json) {
    return ActivationStats(
      totalActivations: json['totalActivations'] ?? 0,
      successful: json['successful'] ?? 0,
      failed: json['failed'] ?? 0,
      pendingPayments: json['pendingPayments'] ?? 0,
    );
  }
}

class ActivationEvent {
  final String pid;
  final String qrId;
  final String? owner;
  final String activatedAt;

  ActivationEvent({
    required this.pid,
    required this.qrId,
    this.owner,
    required this.activatedAt,
  });

  factory ActivationEvent.fromJson(Map<String, dynamic> json) {
    return ActivationEvent(
      pid: json['pid'] ?? '',
      qrId: json['qrId'] ?? '',
      owner: json['owner'],
      activatedAt: json['activatedAt'] ?? '',
    );
  }
}

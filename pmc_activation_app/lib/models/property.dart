class Property {
  final String pid;
  final String ownerName;
  final String guardianName;
  final String mobileNo;
  final String address;
  final String totalDues;
  final String paymentStatus;
  final String circle;
  final String revenueCircleNo;
  final String wardNo;

  Property({
    required this.pid,
    required this.ownerName,
    required this.guardianName,
    required this.mobileNo,
    required this.address,
    required this.totalDues,
    required this.paymentStatus,
    required this.circle,
    required this.revenueCircleNo,
    required this.wardNo,
  });

  factory Property.fromJson(Map<String, dynamic> json) {
    return Property(
      pid: json['pid'] as String? ?? '',
      ownerName: json['ownerName'] as String? ?? '',
      guardianName: json['guardianName'] as String? ?? '',
      mobileNo: json['mobileNo'] as String? ?? '',
      address: json['address'] as String? ?? '',
      totalDues: json['totalDues'] as String? ?? '0',
      paymentStatus: json['paymentStatus'] as String? ?? 'Pending',
      circle: json['circle'] as String? ?? '',
      revenueCircleNo: json['revenueCircleNo'] as String? ?? '',
      wardNo: json['wardNo'] as String? ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'pid': pid,
      'ownerName': ownerName,
      'guardianName': guardianName,
      'mobileNo': mobileNo,
      'address': address,
      'totalDues': totalDues,
      'paymentStatus': paymentStatus,
      'circle': circle,
      'revenueCircleNo': revenueCircleNo,
      'wardNo': wardNo,
    };
  }
}

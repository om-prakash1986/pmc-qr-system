class StaffUser {
  final int staffId;
  final String loginId;
  final String fullName;
  final String role;
  final int? wardNo;
  final String circle;
  final String mobile;

  StaffUser({
    required this.staffId,
    required this.loginId,
    required this.fullName,
    required this.role,
    this.wardNo,
    required this.circle,
    required this.mobile,
  });

  factory StaffUser.fromJson(Map<String, dynamic> json) {
    return StaffUser(
      staffId: json['staffId'] is int
          ? json['staffId']
          : int.tryParse(json['staffId']?.toString() ?? '0') ?? 0,
      loginId: json['loginId'] as String,
      fullName: json['fullName'] as String,
      role: json['role'] as String,
      wardNo: json['wardNo'] as int?,
      circle: json['circle'] as String? ?? '',
      mobile: json['mobile'] as String? ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'staffId': staffId,
      'loginId': loginId,
      'fullName': fullName,
      'role': role,
      'wardNo': wardNo,
      'circle': circle,
      'mobile': mobile,
    };
  }
}

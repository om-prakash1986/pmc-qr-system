class PropertyDetailRow {
  final String pid;
  final String applicationNo;
  final String ownerName;
  final String guardianName;
  final String address;
  final String plotArea;
  final String constructedArea;
  final String assessmentYear;
  final String streetType;
  final String floorNo;
  final String builtupArea;
  final String useType;
  final String usageType;
  final String contructionType;
  final String occupancyType;
  final String mobileNo;
  final String revenueCircleNo;
  final String circle;
  final String ward;
  final String totalDues;

  PropertyDetailRow({
    required this.pid,
    required this.applicationNo,
    required this.ownerName,
    required this.guardianName,
    required this.address,
    required this.plotArea,
    required this.constructedArea,
    required this.assessmentYear,
    required this.streetType,
    required this.floorNo,
    required this.builtupArea,
    required this.useType,
    required this.usageType,
    required this.contructionType,
    required this.occupancyType,
    required this.mobileNo,
    required this.revenueCircleNo,
    required this.circle,
    required this.ward,
    required this.totalDues,
  });

  factory PropertyDetailRow.fromJson(Map<String, dynamic> json) {
    return PropertyDetailRow(
      pid: json['pid']?.toString() ?? '',
      applicationNo: json['application_no']?.toString() ?? '',
      ownerName: json['owner_name']?.toString() ?? '',
      guardianName: json['guardian_name']?.toString() ?? '',
      address: json['address']?.toString() ?? '',
      plotArea: json['plot_area']?.toString() ?? '',
      constructedArea: json['constructed_area']?.toString() ?? '',
      assessmentYear: json['assessment_year']?.toString() ?? '',
      streetType: json['street_type']?.toString() ?? '',
      floorNo: json['floor_no']?.toString() ?? '',
      builtupArea: json['builtup_area']?.toString() ?? '',
      useType: json['use_type']?.toString() ?? '',
      usageType: json['usage_type']?.toString() ?? '',
      contructionType: json['contruction_type']?.toString() ?? '',
      occupancyType: json['occupancy_type']?.toString() ?? '',
      mobileNo: json['mobile_no']?.toString() ?? '',
      revenueCircleNo: json['revenue_circle_no']?.toString() ?? '',
      circle: json['circle']?.toString() ?? '',
      ward: json['ward']?.toString() ?? '',
      totalDues: json['total_dues']?.toString() ?? '0',
    );
  }
}

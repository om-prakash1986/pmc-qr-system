class TaxHistoryRow {
  final String finYear;
  final String propertyId;
  final String floorTax;
  final String vacantTax;
  final String totalTax;

  TaxHistoryRow({
    required this.finYear,
    required this.propertyId,
    required this.floorTax,
    required this.vacantTax,
    required this.totalTax,
  });

  factory TaxHistoryRow.fromJson(Map<String, dynamic> json) {
    return TaxHistoryRow(
      finYear: json['fin_year']?.toString() ?? '',
      propertyId: json['property_id']?.toString() ?? '',
      floorTax: json['floor_tax']?.toString() ?? '0.00',
      vacantTax: json['vacant_tax']?.toString() ?? '0.00',
      totalTax: json['total_tax']?.toString() ?? '0.00',
    );
  }
}

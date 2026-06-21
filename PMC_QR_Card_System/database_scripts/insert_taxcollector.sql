-- Insert a tax collector user into tbl_Staff
-- Adjust column names/types as needed for your schema
INSERT INTO tbl_Staff (Staff_Id, Staff_Name, Username, Password, Role)
VALUES (2001, 'Tax Collector', 'tax101', 'taxpass', 'TAX_COLLECTOR');

-- You can verify insertion with:
-- SELECT * FROM tbl_Staff WHERE Username = 'tax101';

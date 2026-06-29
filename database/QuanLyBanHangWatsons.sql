create database QuanLyBanHangWatsons
go
use QuanLyBanHangWatsons
go

create table tblChiNhanh (
	MaChiNhanh			nchar(4)		not null	constraint PK_tblChiNhanh primary key,
	TenChiNhanh			nvarchar(50)	not null, 
	DiaChi				nvarchar(150)	null
);

create table tblNhanVien (
	MaNhanVien			nchar(6)		not null	constraint PK_tblNhanVien primary key, 
	HoNhanVien			nvarchar(20)	not null, 
	TenNhanVien			nvarchar(20)	not null,
	MaChiNhanh			nchar(4)		not null,
	NgaySinh			date			null,
	GioiTinh			nvarchar(5)		null,
	DienThoai			nvarchar(10)	not null, 
	ChucVu				nvarchar(30)	not null,
	NgayVaoLam			date			not null, 
	Email				nvarchar(30)	not null,
	NgayCapNhat			datetime		not null,
	constraint FK_tbltblNhanVien_MaChiNhanh foreign key (MaChiNhanh) references tblChiNhanh(MaChiNhanh)
);

create table tblTaiKhoanNV (
	MaTaiKhoanNV		nchar(8)		not null	constraint PK_tblTaiKhoanNV primary key,
	TenTaiKhoan			nvarchar(20)	not null,
	MaNhanVien			nchar(6)		not null,
	MatKhau				nvarchar(50)	not null,
	Quyen				nvarchar(30)	not null,
	HinhAnh				nvarchar(255)	null,
	TrangThaiTaiKhoan	bit default 1	not null, -- 1: đang hoạt động / 0: vô hiệu hóa
	NgayCapNhat			datetime		not null,
	constraint FK_tblTaiKhoanNV_MaNhanVien foreign key (MaNhanVien) references tblNhanVien(MaNhanVien)
);

create table tblKhachHang (
	MaKhachHang			nchar(6)		not null	constraint PK_tblKhachHang primary key, 
	HoKhachHang			nvarchar(20)	null, 
	TenKhachHang		nvarchar(20)	null,
	LoaiKhachHang		nvarchar(10)	not null, -- 'Thành viên' hoặc Vãng lai'
	NgaySinh			date			null,
	GioiTinh			nvarchar(5)		null,
	DienThoai			nvarchar(10)	null, 
	DiaChi				nvarchar(150)	null, 
	Email				nvarchar(30)	null,
	NgayDangKy			date			null,
    DiemTichLuy			int default 0	null,
	HangThanhVien		nvarchar(20)	default N'Watsons Club' null,
	MaNhanVien			nchar(6)		not null,
	NgayCapNhat			datetime		not null,
	constraint FK_tblKhachHang_MaNhanVien foreign key (MaNhanVien) references tblNhanVien (MaNhanVien)
);

create table tblTaiKhoanKH (
	MaTaiKhoanKH		nchar(8)		not null	constraint PK_tblTaiKhoanKH primary key,
	TenTaiKhoan			nvarchar(20)	not null,
	MaKhachHang			nchar(6)		not null,
	MatKhau				nvarchar(50)	not null,
	Quyen				nvarchar(15)	not null,
	HinhAnh				nvarchar(255)	null,
	TrangThaiTaiKhoan	bit default 1	not null, -- 1: đang hoạt động / 0: vô hiệu hóa
	NgayCapNhat			datetime		not null,
	constraint FK_tblTaiKhoanKH_MaKhachHang foreign key (MaKhachHang) references tblKhachHang(MaKhachHang)
);

create table tblNhaCungCap (
	MaNhaCungCap		nchar(6)		not null	constraint PK_tblNhaCungCap primary key,
	TenNhaCungCap		nvarchar(60)	not null,
	DiaChi				nvarchar(150)	not null,
	DienThoai			nvarchar(15)	not null,
	Email				nvarchar(40)	not null
);

create table tblLoaiSanPham (
	MaLoaiSanPham		nchar(6)		not null	constraint PK_tblLoaiSanPham	primary key, 
	TenLoaiSanPham		nvarchar(50)	not null
);

create table tblThuongHieu (
	MaThuongHieu		nchar(6)		not null	constraint PK_tblThuongHieu	primary key, 
	TenThuongHieu		nvarchar(50)	not null
);

create table tblSanPham (
	MaSanPham			nchar(6)		not null	constraint PK_tblSanPham primary key,
	TenSanPham			nvarchar(150)	not null,
	MaLoaiSanPham		nchar(6)		not null,
	MaThuongHieu		nchar(6)		not null,
	DonViTinh			nvarchar(30)	null,
	GiaBan				decimal(18,2)	not null,
	MoTa				nvarchar(255)	null,
	TrangThaiSanPham	bit default 1,				-- 1: đang bán / 0: ngừng kinh doanh
	HinhAnh				nvarchar(255)	null,
	MaVach				varchar(13)		not null unique,
	MaNhaCungCap		nchar(6)		not null,
	MaNhanVien			nchar(6)		not null,
	NgayCapNhat			datetime		not null,
	constraint FK_tblSanPham_MaLoaiSanPham foreign key (MaLoaiSanPham) references tblLoaiSanPham(MaLoaiSanPham),
	constraint FK_tblSanPham_MaNhaCungCap foreign key (MaNhaCungCap) references tblNhaCungCap(MaNhaCungCap),
	constraint FK_tblSanPham_MaThuongHieu foreign key (MaThuongHieu) references tblThuongHieu(MaThuongHieu),
	constraint FK_tblSanPham_MaNhanVien foreign key (MaNhanVien) references tblNhanVien(MaNhanVien)
);

create table tblKhuyenMai (
	MaKhuyenMai			nchar(6)		not null	constraint PK_tlbKhuyenMai primary key,
	TenKhuyenMai		nvarchar(100)	not null,
	GiaTri				decimal(18,2)	not null,
	MoTa				nvarchar(255)	null,
	GiaTriDonHang		decimal(18,2)	null,
	LoaiDonHangApDung	nvarchar(20)	null,	-- Tại cửa hàng/Trực tuyến/Tất cả
    HangThanhVienApDung nvarchar(20)    null,	-- N'Watsons Club' , N'Watsons Club Elite' , N'Tất cả'
	ApDungThangSinhNhat bit default 0,			-- 1: áp dụng tháng sinh nhật
	ApDungKhachHangMoi  bit default 0,			-- 1: khách mới
	MaSanPham			nchar(6)		null,
	MaLoaiSanPham		nchar(6)		null,
	MaThuongHieu		nchar(6)		null,
	NgayBatDau			date			null,
	NgayKetThuc			date			null,
	SoLanSuDungToiDa	int				not null,
	TrangThaiKhuyenMai	bit default 1,			-- 1: đang áp dụng / 0: hết hạn
	MaNhanVien			nchar(6)		not null,
	constraint FK_tblKhuyenMai_MaNhanVien foreign key (MaNhanVien) references tblNhanVien(MaNhanVien),
	constraint FK_tblKhuyenMai_MaSanPham foreign key (MaSanPham) references tblSanPham(MaSanPham),
	constraint FK_tblKhuyenMai_MaLoaiSanPham foreign key (MaLoaiSanPham) references tblLoaiSanPham(MaLoaiSanPham),
	constraint FK_tblKhuyenMai_MaThuongHieu foreign key (MaThuongHieu) references tblThuongHieu(MaThuongHieu)
);

create table tblMaKMCuaKH (
    MaKhachHang     nchar(6)  not null,
    MaKhuyenMai     nchar(6)  not null,
    SoLanDaSuDung   int       not null default 0,
    constraint PK_tblKhachHang_KhuyenMai primary key (MaKhachHang, MaKhuyenMai),
    constraint PK_KH_KM_KhachHang foreign key (MaKhachHang) references tblKhachHang(MaKhachHang),
    constraint PK_KH_KM_KhuyenMai foreign key (MaKhuyenMai) references tblKhuyenMai(MaKhuyenMai)
);

create table tblDonHang (
	MaDonHang			nchar(6)		not null	constraint PK_tblDonHang primary key,
	MaKhachHang			nchar(6)		not null, 
	MaNhanVien          nchar(6)        not null,
	LoaiDonHang			nvarchar(20)	not null, -- Tại cửa hàng/Trực tuyến
	NgayDatHang			datetime		not null,
	TrangThaiDonHang	nvarchar(50)	not null, -- 
	TongTienHang		decimal(18,2)	not null, --note
	MaKhuyenMai			nchar(6)		null,
	DiemSuDung			int	default 0	null,
	ThanhTien			decimal(18,2)	not null, --note
	MaChiNhanh			nchar(4)		null,
	GhiChu				nvarchar(100)	null,
	constraint FK_tblDonHang_MaKhachHang foreign key (MaKhachHang) references tblKhachHang(MaKhachHang),
	constraint FK_tblDonHang_MaNhanVien foreign key (MaNhanVien) references tblNhanVien(MaNhanVien),
	constraint FK_tblDonHang_MaKhuyenMai foreign key (MaKhuyenMai) references tblKhuyenMai(MaKhuyenMai),
	constraint FK_tblDonHang_MaChiNhanh foreign key (MaChiNhanh) references tblChiNhanh(MaChiNhanh),
);

create table tblChiTietDonHang (
	MaDonHang			nchar(6)		not null, 
	MaSanPham			nchar(6)		not null, 
	SoLuong				int				not null, 
	DonGia				decimal(18,2)	not null, 
	constraint PK_tblChiTietDonHang_MaDonHang foreign key (MaDonHang) references tblDonHang (MaDonHang),
	constraint PK_tblChiTietDonHang_MaSanPham foreign key (MaSanPham) references tblSanPham (MaSanPham),
	constraint PK_tblChiTietDonHang primary key (MaDonHang, MaSanPham),
);

create table tblChiTietDiemTichLuy (
	MaKhachHang			nchar(6)		not null,
	MaDonHang			nchar(6)		not null,
	DiemDaSuDung		int				null,
	DiemTichLuy			int default 0	not null,
	NgayHetHan			date			not null,
	constraint PK_tblChiTietDiemTichLuy_MaDonHang foreign key (MaDonHang) references tblDonHang (MaDonHang),
	constraint PK_tblChiTietDiemTichLuy_MaKhachHang foreign key (MaKhachHang) references tblKhachHang (MaKhachHang),
	constraint PK_tblChiTietDiemTichLuy primary key (MaDonHang, MaKhachHang)
);

create table tblThanhToan (
	MaThanhToan			nchar(6)		not null	constraint PK_tblThanhToan primary key,
	MaKhachHang			nchar(6)		not null, 
	MaDonHang			nchar(6)		not null,
	MaNhanVien          nchar(6)        not null,
	PhuongThucThanhToan nvarchar(30)	not null, --Thẻ tín dụng/Thẻ ghi nợ/Chuyển khoản/ Thẻ ATM/Tiền mặt khi giao hàng/Thanh toán QR/Ví điện tử
	NgayThanhToan		datetime		null,
	TongTien			decimal(18,2)	not null, ----note
	TrangThaiThanhToan	bit				not null, -- 1: đã thanh toán / 0: chưa thanh toán
	constraint FK_tblThanhToan_MaKhachHang foreign key (MaKhachHang) references tblKhachHang(MaKhachHang),
	constraint FK_tblThanhToan_MaDonHang foreign key (MaDonHang) references tblDonHang(MaDonHang),
	constraint FK_tblThanhToan_MaNhanVien foreign key (MaNhanVien) references tblNhanVien(MaNhanVien)
);

create table tblHoaDon (
	MaHoaDon			nchar(6)		not null	constraint PK_tblHoaDon primary key,
	MaThanhToan			nchar(6)		not null,
	MaNhanVien          nchar(6)        not null,
	NgayXuatHoaDon		datetime		not null,
	TongTien			decimal(18,2)	not null, --note
	SoTienDaThanhToan	decimal(18,2)	not null,--note
	SoDuConLai			decimal(18,2)	not null,--note
	constraint FK_tblHoaDon_MaThanhToan foreign key (MaThanhToan) references tblThanhToan(MaThanhToan),
	constraint FK_tblHoaDon_MaNhanVien foreign key (MaNhanVien) references tblNhanVien(MaNhanVien)
);

create table tblPhieuNhap (
	MaPhieuNhap			nchar(6)		not null	constraint PK_tlbPhieuNhap primary key,
	MaNhanVien			nchar(6)		not null,
	MaNhaCungCap		nchar(6)		not null,
	MaChiNhanh			nchar(4)		not null,
	NgayNhap			date			not null,
	TongTien			decimal(18,2)	not null,--note
	GhiChu				nvarchar(250)	null,
	constraint FK_tblPhieuNhap_MaNhanVien foreign key (MaNhanVien) references tblNhanVien(MaNhanVien),
	constraint FK_tblPhieuNhap_MaNhaCungCap foreign key (MaNhaCungCap) references tblNhaCungCap(MaNhaCungCap),
	constraint FK_tblPhieuNhap_MaChiNhanh foreign key (MaChiNhanh) references tblChiNhanh(MaChiNhanh)
);

create table tblCTPhieuNhap (
	MaPhieuNhap			nchar(6)		not null,
	MaSanPham			nchar(6)		not null,
	SoLuong				int				not null,
	GiaNhap				decimal(18,2)	not null,
	NgaySanXuat			date            not null,
    HanSuDung			date            not null,
	constraint PK_tblCTPhieuNhap_MaPhieuNhap foreign key (MaPhieuNhap) references tblPhieuNhap (MaPhieuNhap),
	constraint PK_tblCTPhieuNhap_MaSanPham foreign key (MaSanPham) references tblSanPham (MaSanPham),
	constraint PK_tblCTPhieuNhap primary key (MaPhieuNhap, MaSanPham)
);

create table tblPhieuXuat (
	MaPhieuXuat			nchar(6)		not null	constraint PK_tlbPhieuXuat primary key,
	MaNhanVien			nchar(6)		not null,
	MaDonHang			nchar(6)		not null, 
	MaChiNhanh			nchar(4)		not null,
	NgayXuat			date			not null,
	TongTien			decimal(18,2)	not null,--note
	GhiChu				nvarchar(250)	null,
	constraint FK_tblPhieuXuat_MaNhanVien foreign key (MaNhanVien) references tblNhanVien (MaNhanVien),
	constraint FK_tblPhieuXuat_MaDonHang foreign key (MaDonHang) references tblDonHang (MaDonHang),
	constraint FK_tblPhieuXuat_MaChiNhanh foreign key (MaChiNhanh) references tblChiNhanh (MaChiNhanh)
);

create table tblCTPhieuXuat (
	MaPhieuXuat			nchar(6)		not null,
	MaSanPham			nchar(6)		not null,
	SoLuong				int				not null,
	DonGia				decimal(18,2)	not null,
	constraint PK_tblCTPhieuXuat_MaPhieuXuat foreign key (MaPhieuXuat) references tblPhieuXuat (MaPhieuXuat),
	constraint PK_tblCTPhieuXuat_MaSanPham foreign key (MaSanPham) references tblSanPham (MaSanPham),
	constraint PK_tblCTPhieuXuat primary key (MaPhieuXuat, MaSanPham)
);

create table tblTonKho (
	MaTonKho			nchar(6)		not null	constraint PK_tblTonKho primary key,
	MaSanPham			nchar(6)		not null,
	MaChiNhanh			nchar(4)		not null,
	MaNhanVien			nchar(6)		not null,
	SoLuongSanBan		int				not null,
	SoLuongTon			int				not null,
	NgayCapNhat			date			not null,
	constraint FK_tblTonKho_MaChiNhanh foreign key (MaChiNhanh) references tblChiNhanh(MaChiNhanh),
    constraint FK_tblTonKho_MaSanPham foreign key (MaSanPham) references tblSanPham(MaSanPham),
	constraint FK_tblTonKho_MaNhanVien foreign key (MaNhanVien) references tblNhanVien (MaNhanVien)
);

	
									--------------------------------------------------------------------
                                                        -- BẢNG CHI NHÁNH --
                                    --------------------------------------------------------------------
--Dữ liệu chi nhánh
insert into tblChiNhanh (MaChiNhanh, TenChiNhanh, DiaChi)
values	('CN00', N'Trung tâm điều hành', null),
		('CN01', N'AEON Bình Tân', N'Số 1, Đường 17A Phường An Lạc Hồ Chí Minh'),
		('CN02', N'AEON Tân Phú', N'30 Bờ Bao Tân Thắng Phường Tây Thạnh Hồ Chí Minh'),
		('CN03', N'Bitexco', N'2 Hải Triều Phường Sài Gòn Hồ Chí Minh'),
		('CN04', N'Central Premium', N'L1-02A, 854-856 Tạ Quang Bửu Phường Chánh Hưng Hồ Chí Minh'),
		('CN05', N'E-mart Phan Văn Trị', N'366 Phan Văn Trị Phường An Nhơn Hồ Chí Minh'),
		('CN06', N'Kho trung tâm', null);

---------------------------------------------------------------------
-- TRIGGER: Quản lý INSERT/UPDATE cho tblChiNhanh
---------------------------------------------------------------------
CREATE OR ALTER TRIGGER trg_NhapChiNhanh_InsertUpdate
ON tblChiNhanh
INSTEAD OF INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Ma nchar(4), @Ten nvarchar(50), @DiaChi nvarchar(150);

    SELECT @Ma = MaChiNhanh, @Ten = TenChiNhanh, @DiaChi = DiaChi
    FROM inserted;

    -----------------------------------------------------------------
    -- Kiểm tra TenChiNhanh
    -----------------------------------------------------------------
    IF (@Ten LIKE '[0-9]%' OR @Ten LIKE '%[^a-zA-Z0-9 ]%')
    BEGIN
        RAISERROR(N'Tên chi nhánh không hợp lệ! Không được bắt đầu bằng số và không chứa ký tự đặc biệt.', 16, 1);
        RETURN;
    END;

    -- Viết hoa chữ cái đầu tiên
    SET @Ten = UPPER(LEFT(@Ten, 1)) + LOWER(SUBSTRING(@Ten, 2, LEN(@Ten)));

    -----------------------------------------------------------------
    -- Kiểm tra Địa chỉ
    -----------------------------------------------------------------
    IF @DiaChi LIKE '%[^A-Za-z0-9/., ]%'
    BEGIN
        RAISERROR (N'Địa chỉ chỉ được chứa chữ, số, dấu "/ . ," và khoảng trắng.', 16, 1);
        RETURN;
    END;

    IF LEFT(@DiaChi,1) NOT LIKE '[A-Za-z0-9]'
    BEGIN
        RAISERROR (N'Địa chỉ phải bắt đầu bằng chữ hoặc số.', 16, 1);
        RETURN;
    END;

    -- Dấu "/" phải nằm giữa hai số
    IF @DiaChi LIKE '%[^0-9]/%' OR @DiaChi LIKE '%/[^0-9]%' 
       OR @DiaChi LIKE '/%' OR @DiaChi LIKE '%/'
    BEGIN
        RAISERROR (N'Địa chỉ không hợp lệ: dấu "/" phải nằm giữa hai chữ số.', 16, 1);
        RETURN;
    END;


    -----------------------------------------------------------------
    -- Phân biệt INSERT và UPDATE
    -----------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM tblChiNhanh WHERE MaChiNhanh = @Ma)
    BEGIN
        -- INSERT: tự sinh mã mới
        DECLARE @LastCode nchar(4) = (
            SELECT TOP 1 MaChiNhanh
            FROM tblChiNhanh
            ORDER BY MaChiNhanh DESC
        );

        DECLARE @NewNumber int, @NewCode nchar(4);

        IF (@LastCode IS NULL)
            SET @NewNumber = 1;
        ELSE
            SET @NewNumber = CAST(SUBSTRING(@LastCode, 3, 2) AS int) + 1;

        SET @NewCode = 'CN' + RIGHT('0' + CAST(@NewNumber AS varchar(2)), 2);

        INSERT INTO tblChiNhanh (MaChiNhanh, TenChiNhanh, DiaChi)
        VALUES (@NewCode, @Ten, @DiaChi);
    END
    ELSE
    BEGIN
        -- UPDATE: cấm sửa mã chi nhánh
        IF UPDATE(MaChiNhanh)
        BEGIN
            RAISERROR(N'Không được phép sửa mã chi nhánh!', 16, 1);
            RETURN;
        END;

        UPDATE tblChiNhanh
        SET TenChiNhanh = @Ten,
            DiaChi = @DiaChi
        WHERE MaChiNhanh = @Ma;
    END;
END;
GO




									--------------------------------------------------------------------
                                                        -- BẢNG NHÂN VIÊN --
                                    --------------------------------------------------------------------
-- Dữ liệu Nhân viên 
insert into tblNhanVien (MaNhanVien, HoNhanVien, TenNhanVien, MaChiNhanh, NgaySinh, GioiTinh, DienThoai, ChucVu, NgayVaoLam, Email, NgayCapNhat)
values	('NV0001', N'Nguyễn', N'Văn An', 'CN01','1990-04-12', N'Nam', '0912345678', N'Nhân viên bán hàng', '2020-03-01', 'an.nguyen@gmail.com', '2025-01-01'),
		('NV0002', N'Lê', N'Thị Bích Hương', 'CN01', '1992-05-20', N'Nữ', '0913456789', N'Nhân viên bán hàng', '2021-07-15', 'huong.le@gmail.com', '2025-01-01'),
		('NV0003', N'Trần', N'Hữu Duy', 'CN01', '1989-08-11', N'Nam', '0914567890', N'Nhân viên bán hàng', '2022-01-20', 'duy.tran@gmail.com', '2025-01-01'),
		('NV0004', N'Phạm', N'Ngọc Mai', 'CN01', '1993-01-25', N'Nữ', '0915678901', N'Nhân viên bán hàng', '2023-05-10', 'mai.pham@gmail.com', '2025-01-01'),
		('NV0005', N'Vũ', N'Thị Lan', 'CN01','1988-09-09', N'Nữ', '0916789012', N'Nhân viên bán hàng', '2019-06-18', 'lan.vu@gmail.com', '2025-01-01'),
		('NV0006', N'Đặng', N'Minh Quân', 'CN01', '1991-07-07', N'Nam', '0917890123', N'Nhân viên bán hàng', '2020-12-22', 'quan.dang@gmail.com', '2025-01-01'),
		('NV0007', N'Ngô', N'Thị Thanh', 'CN01', '1994-11-30', N'Nữ', '0918901234', N'Nhân viên kho', '2018-08-01', 'thanh.ngo@gmail.com', '2025-01-01'),
		('NV0008', N'Bùi', N'Quốc Tuấn', 'CN01', '1990-06-03', N'Nam', '0919012345', N'Quản lý cửa hàng', '2021-03-20', 'tuan.bui@gmail.com', '2025-01-01'),
		('NV0009', N'Hoàng', N'Thị Hương', 'CN01', '1992-02-14', N'Nữ', '0920123456', N'Quản lý kho', '2022-06-01', 'huong.hoang@gmail.com', '2025-01-01'),
		('NV0010', N'Đỗ', N'Anh Cường', 'CN02', '1991-05-05', N'Nam', '0921234567', N'Nhân viên bán hàng', '2023-03-18', 'cuong.do@gmail.com', '2025-01-08'),
		('NV0011', N'Lê', N'Thị Mai', 'CN02', '1993-08-22', N'Nữ', '0922345678', N'Nhân viên bán hàng', '2021-10-15', 'mai.le@gmail.com', '2025-01-08'),
		('NV0012', N'Nguyễn', N'Văn Bình', 'CN02', '1989-12-14', N'Nam', '0923456789', N'Nhân viên bán hàng', '2020-04-10', 'binh.nguyen@gmail.com', '2025-01-08'),
		('NV0013', N'Trần', N'Thị Thảo', 'CN02', '1995-01-19', N'Nữ', '0924567890', N'Nhân viên bán hàng', '2022-01-25', 'thao.tran@gmail.com', '2025-01-08'),
		('NV0014', N'Phạm', N'Quốc Duy', 'CN02', '1990-09-28', N'Nam', '0925678901', N'Nhân viên bán hàng', '2019-11-12', 'duy.pham@gmail.com', '2025-01-08'),
		('NV0015', N'Hoàng', N'Thị Ngọc', 'CN02', '1994-03-05', N'Nữ', '0926789012', N'Nhân viên bán hàng', '2023-05-20', 'ngoc.hoang@gmail.com', '2025-01-08'),
		('NV0016', N'Vũ', N'Thành An', 'CN02', '1991-07-01', N'Nam', '0927890123', N'Nhân viên kho', '2021-09-10', 'an.vu@gmail.com', '2025-01-08'),
		('NV0017', N'Bùi', N'Thị Lan', 'CN02', '1992-10-20', N'Nữ', '0928901234', N'Quản lý cửa hàng', '2022-03-14', 'lan.bui@gmail.com', '2025-01-08'),
		('NV0018', N'Ngô', N'Hữu Hùng', 'CN02', '1988-06-08', N'Nam', '0929012345', N'Quản lý kho', '2020-02-28', 'hung.ngo@gmail.com', '2025-01-08'),
		('NV0019', N'Đỗ', N'Minh Tâm', 'CN03', '1993-02-17', N'Nữ', '0930123456', N'Nhân viên bán hàng', '2023-06-15', 'tam.do@gmail.com', '2025-01-08'),
		('NV0020', N'Đặng', N'Thị Hằng', 'CN03', '1996-11-23', N'Nữ', '0931234567', N'Nhân viên bán hàng', '2022-09-05', 'hang.dang@gmail.com', '2025-01-15'),
		('NV0021', N'Nguyễn', N'Hữu Phúc', 'CN03', '1992-04-18', N'Nam', '0932345678', N'Nhân viên bán hàng', '2021-01-11', 'phuc.nguyen@gmail.com', '2025-01-15'),
		('NV0022', N'Lê', N'Thị Ngọc', 'CN03', '1994-12-03', N'Nữ', '0933456789', N'Nhân viên bán hàng', '2023-04-17', 'ngoc.le@gmail.com', '2025-01-15'),
		('NV0023', N'Trần', N'Anh Khoa', 'CN03', '1991-06-06', N'Nam', '0934567890', N'Nhân viên bán hàng', '2020-09-25', 'khoa.tran@gmail.com', '2025-01-15'),
		('NV0024', N'Phạm', N'Thị Bích Hà', 'CN03', '1995-03-30', N'Nữ', '0935678901', N'Nhân viên kho', '2022-07-02', 'ha.pham@gmail.com', '2025-01-15'),
		('NV0025', N'Vũ', N'Văn Cường', 'CN03', '1990-01-20', N'Nam', '0936789012', N'Quản lý cửa hàng', '2021-05-13', 'cuong.vu@gmail.com', '2025-01-15'),
		('NV0026', N'Hoàng', N'Thị Tuyết', 'CN03', '1993-09-17', N'Nữ', '0937890123', N'Quản lý kho', '2023-01-19', 'tuyet.hoang@gmail.com', '2025-01-15'),
		('NV0027', N'Bùi', N'Thanh Sơn', 'CN04', '1991-10-08', N'Nam', '0938901234', N'Nhân viên bán hàng', '2022-10-10', 'son.bui@gmail.com', '2025-01-15'),
		('NV0028', N'Ngô', N'Ngọc Linh', 'CN04', '1996-02-12', N'Nữ', '0939012345', N'Nhân viên bán hàng', '2023-06-28', 'linh.ngo@gmail.com', '2025-01-15'),
		('NV0029', N'Đỗ', N'Thành Trung', 'CN04', '1989-07-29', N'Nam', '0940123456', N'Nhân viên bán hàng', '2019-12-05', 'trung.do@gmail.com', '2025-01-22'),
		('NV0030', N'Đặng', N'Thị Minh', 'CN04', '1992-08-14', N'Nữ', '0941234567', N'Nhân viên bán hàng', '2021-04-09', 'minh.dang@gmail.com', '2025-01-22'),
		('NV0031', N'Nguyễn', N'Quốc Hưng', 'CN04', '1990-06-15', N'Nam', '0942345678', N'Nhân viên bán hàng', '2021-02-14', 'hung.nguyen@gmail.com', '2025-01-22'),
		('NV0032', N'Lê', N'Thị Tâm', 'CN04', '1995-11-02', N'Nữ', '0943456789', N'Nhân viên kho', '2022-05-10', 'tam.le@gmail.com', '2025-01-22'),
		('NV0033', N'Trần', N'Văn Dũng', 'CN04', '1993-03-08', N'Nam', '0944567890', N'Quản lý cửa hàng', '2020-06-30', 'dung.tran@gmail.com', '2025-01-22'),
		('NV0034', N'Phạm', N'Thị Yến', 'CN04', '1994-04-26', N'Nữ', '0945678901', N'Quản lý kho', '2023-03-01', 'yen.pham@gmail.com', '2025-01-22'),
		('NV0035', N'Vũ', N'Minh Đạt', 'CN05', '1992-10-19', N'Nam', '0946789012', N'Nhân viên bán hàng', '2021-08-21', 'dat.vu@gmail.com', '2025-01-22'),
		('NV0036', N'Hoàng', N'Thị Kim', 'CN05', '1991-07-12', N'Nữ', '0947890123', N'Nhân viên bán hàng', '2022-11-03', 'kim.hoang@gmail.com', '2025-01-22'),
		('NV0037', N'Bùi', N'Văn Hào', 'CN05', '1990-09-05', N'Nam', '0948901234', N'Nhân viên bán hàng', '2023-02-09', 'hao.bui@gmail.com', '2025-01-22'),
		('NV0038', N'Ngô', N'Thị Lan', 'CN05', '1993-05-17', N'Nữ', '0949012345', N'Nhân viên bán hàng', '2021-07-19', 'lan.ngo@gmail.com', '2025-01-22'),
		('NV0039', N'Đỗ', N'Anh Tú', 'CN05', '1989-01-27', N'Nam', '0950123456', N'Nhân viên bán hàng', '2019-03-22', 'tu.do@gmail.com', '2025-01-29'),
		('NV0040', N'Đặng', N'Thị Thùy', 'CN05', '1995-06-11', N'Nữ', '0951234567', N'Nhân viên kho', '2020-05-01', 'thuy.dang@gmail.com', '2025-01-29'),
		('NV0041', N'Nguyễn', N'Thị Hồng', 'CN05', '1994-03-13', N'Nữ', '0952345678', N'Quản lý cửa hàng', '2020-04-17', 'hong.nguyen@gmail.com', '2025-01-29'),
		('NV0042', N'Lê', N'Thanh Hải', 'CN05', '1992-12-29', N'Nam', '0953456789', N'Quản lý kho', '2021-06-20', 'hai.le@gmail.com', '2025-01-29'),
		('NV0043', N'Trần', N'Thị Lệ', 'CN00', '1995-08-06', N'Nữ', '0954567890', N'Quản trị viên', '2022-10-14', 'le.tran@gmail.com', '2025-01-29'),
		('NV0044', N'Phạm', N'Hoàng Nam', 'CN00', '1991-01-11', N'Nam', '0955678901', N'Quản trị viên', '2020-09-01', 'nam.pham@gmail.com', '2025-01-29'),
		('NV0045', N'Vũ', N'Thị Bảo', 'CN00', '1996-07-09', N'Nữ', '0956789012', N'Quản trị viên', '2023-04-05', 'bao.vu@gmail.com', '2025-01-29'),
		('NV0046', N'Hoàng', N'Thế Vinh', 'CN00', '1989-05-15', N'Nam', '0957890123', N'Quản lý cửa hàng tổng', '2021-02-16', 'vinh.hoang@gmail.com', '2025-01-29'),
		('NV0047', N'Bùi', N'Thị Thu', 'CN06', '1993-10-30', N'Nữ', '0958901234', N'Nhân viên tổng kho', '2022-08-12', 'thu.bui@gmail.com', '2025-01-29'),
		('NV0048', N'Ngô', N'Trọng Tín', 'CN06', '1990-02-08', N'Nam', '0959012345', N'Nhân viên tổng kho', '2020-12-01', 'tin.ngo@gmail.com', '2025-01-29'),
		('NV0049', N'Đỗ', N'Thị Loan', 'CN06', '1994-06-25', N'Nữ', '0960123456', N'Nhân viên tổng kho', '2021-07-07', 'loan.do@gmail.com', '2025-01-29'),
		('NV0050', N'Đặng', N'Phương Thảo', 'CN06', '1995-09-03', N'Nữ', '0961234567', N'Quản lý kho tổng', '2022-06-10', 'thao.dang@gmail.com', '2025-01-29');

---------------------------------------------------------------------
-- TRIGGER XỬ LÝ THÊM / SỬA NHÂN VIÊN
---------------------------------------------------------------------
CREATE TRIGGER trg_NhapNhanVien_Validate
ON tblNhanVien
INSTEAD OF INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NewCode NCHAR(6);

    -----------------------------------------------------------------
    -- 1. TỰ SINH MÃ NHÂN VIÊN (INSERT)
    -----------------------------------------------------------------
    IF EXISTS (SELECT 1 FROM inserted)
       AND NOT EXISTS (SELECT 1 FROM deleted)
    BEGIN
        DECLARE @LastCode NCHAR(6), @Num INT;

        SELECT @LastCode = MAX(MaNhanVien) FROM tblNhanVien;

        IF @LastCode IS NULL
            SET @NewCode = 'NV0001';
        ELSE
        BEGIN
            SET @Num = CAST(SUBSTRING(@LastCode, 3, 4) AS INT) + 1;
            SET @NewCode = 'NV' + RIGHT('0000' + CAST(@Num AS VARCHAR(4)), 4);
        END
    END

    -----------------------------------------------------------------
    -- 2. KIỂM TRA HỌ NHÂN VIÊN (Chỉ chữ + khoảng trắng + Viết hoa chữ cái đầu)
    -----------------------------------------------------------------
    IF EXISTS (
        SELECT 1 FROM inserted
        WHERE HoNhanVien LIKE '%[^a-zA-Z ]%'
           OR HoNhanVien <> 
              STUFF(LOWER(HoNhanVien), 1, 1, UPPER(LEFT(HoNhanVien,1)))
    )
    BEGIN
        RAISERROR(N'Họ nhân viên chỉ được chứa chữ và khoảng trắng, viết hoa chữ cái đầu.', 16, 1);
        RETURN;
    END

    -----------------------------------------------------------------
    -- 3. KIỂM TRA TÊN NHÂN VIÊN (Chỉ chữ + khoảng trắng + Viết hoa chữ cái đầu)
    -----------------------------------------------------------------
    IF EXISTS (
        SELECT 1 FROM inserted
        WHERE TenNhanVien LIKE '%[^a-zA-Z ]%'
           OR TenNhanVien <> 
              STUFF(LOWER(TenNhanVien), 1, 1, UPPER(LEFT(TenNhanVien,1)))
    )
    BEGIN
        RAISERROR(N'Tên nhân viên chỉ được chứa chữ và khoảng trắng, viết hoa chữ cái đầu.', 16, 1);
        RETURN;
    END

    -----------------------------------------------------------------
    -- 4. KIỂM TRA NGÀY SINH (Trên 18 tuổi)
    -----------------------------------------------------------------
    IF EXISTS (
        SELECT 1 FROM inserted
        WHERE NgaySinh IS NULL 
           OR DATEDIFF(YEAR, NgaySinh, GETDATE()) < 18
    )
    BEGIN
        RAISERROR(N'Nhân viên phải trên 18 tuổi.', 16, 1);
        RETURN;
    END

    -----------------------------------------------------------------
    -- 5. KIỂM TRA SỐ ĐIỆN THOẠI (10 chữ số, đầu số hợp lệ)
    -----------------------------------------------------------------
    IF EXISTS (
    SELECT 1 FROM inserted
    WHERE LEN(DienThoai) <> 10
       OR DienThoai LIKE '%[^0-9]%'
       OR LEFT(DienThoai, 3) NOT IN ('032','033','034','035','036','037','038','039',
                                     '050','051','052','053','054','055','056','057','058','059',
                                     '070','071','072','073','074','075','076','077','078','079',
                                     '080','081','082','083','084','085','086','087','088','089',
                                     '090','091','092','093','094','095','096','097','098','099')
    )
    BEGIN
        RAISERROR(N'Số điện thoại phải gồm 10 chữ số và bắt đầu bằng 03x, 05x, 07x, 08x, 09x.', 16, 1);
        RETURN;
    END


    -----------------------------------------------------------------
    -- 6. KIỂM TRA NGÀY VÀO LÀM (>= 18 tuổi)
    -----------------------------------------------------------------
    IF EXISTS (
        SELECT 1 FROM inserted
        WHERE NgayVaoLam IS NULL
           OR DATEDIFF(YEAR, NgaySinh, NgayVaoLam) < 18
    )
    BEGIN
        RAISERROR(N'Ngày vào làm phải sau khi nhân viên đủ 18 tuổi.', 16, 1);
        RETURN;
    END

    -----------------------------------------------------------------
    -- 7. KIỂM TRA EMAIL
    -----------------------------------------------------------------
   IF EXISTS (
        SELECT 1 FROM inserted
        WHERE 
            Email IS NOT NULL AND (
                Email NOT LIKE '%@%.%'                  
                OR Email LIKE '%..%'                    
                OR Email LIKE '.%'                      
                OR Email LIKE '%@.%'                    
                OR Email LIKE '%@-%'                    
                OR Email LIKE '%-.%'                    
                OR Email LIKE '% %'                     
                OR LEN(Email) < 6                       
                OR LEN(Email) > 254                     
                OR Email LIKE '%[^a-zA-Z0-9._%+\-@]%'   
                OR (LEN(Email) - LEN(REPLACE(Email,'@',''))) <> 1   
                OR Email NOT LIKE '%@%.[a-zA-Z][a-zA-Z]%'           
                OR RIGHT(Email,1) = '.'                              
            )
    )
    BEGIN
        RAISERROR(N'Email không hợp lệ.', 16, 1);
        RETURN;
    END

    -----------------------------------------------------------------
    -- 8. INSERT / UPDATE VÀ GHI NHẬT KÝ NGÀY CẬP NHẬT
    -----------------------------------------------------------------
    IF NOT EXISTS (SELECT * FROM deleted)
    BEGIN
        INSERT INTO tblNhanVien
            (MaNhanVien, HoNhanVien, TenNhanVien, MaChiNhanh, NgaySinh,
             GioiTinh, DienThoai, ChucVu, NgayVaoLam, Email, NgayCapNhat)
        SELECT 
            @NewCode,
            HoNhanVien,
            TenNhanVien,
            MaChiNhanh,
            NgaySinh,
            GioiTinh,
            DienThoai,
            ChucVu,
            NgayVaoLam,
            Email,
            GETDATE()
        FROM inserted;
    END
    ELSE
    BEGIN
        UPDATE NV
        SET 
            HoNhanVien = i.HoNhanVien,
            TenNhanVien = i.TenNhanVien,
            MaChiNhanh = i.MaChiNhanh,
            NgaySinh = i.NgaySinh,
            GioiTinh = i.GioiTinh,
            DienThoai = i.DienThoai,
            ChucVu = i.ChucVu,
            NgayVaoLam = i.NgayVaoLam,
            Email = i.Email,
            NgayCapNhat = GETDATE()
        FROM tblNhanVien NV
        INNER JOIN inserted i ON NV.MaNhanVien = i.MaNhanVien;
    END
END
GO


									--------------------------------------------------------------------
                                                        -- BẢNG TK NHÂN VIÊN --
                                    --------------------------------------------------------------------
-- Dữ liệu Tài khoản Nhân viên 
insert into tblTaiKhoanNV (MaTaiKhoanNV, TenTaiKhoan, MaNhanVien, MatKhau, Quyen, HinhAnh, TrangThaiTaiKhoan, NgayCapNhat) 
values	('TKNV0001', 'an_nguyen', 'NV0001', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-01'),
		('TKNV0002', 'huong_le', 'NV0002', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-01'),
		('TKNV0003', 'duy_tran', 'NV0003', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-01'),
		('TKNV0004', 'mai_pham', 'NV0004', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-01'),
		('TKNV0005', 'lan_vu', 'NV0005', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-01'),
		('TKNV0006', 'quan_dang', 'NV0006', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-01'),
		('TKNV0007', 'thanh_ngo', 'NV0007', 'Nnk1234@', N'Nhân viên kho', 'Avatar.png', 1, '2025-01-01'),
		('TKNV0008', 'tuan_bui', 'NV0008', 'Qlch123@', N'Quản lý cửa hàng', 'Avatar.png', 1, '2025-01-01'),
		('TKNV0009', 'huong_hoang', 'NV0009', 'Qlk1234@', N'Quản lý kho', 'Avatar.png', 1, '2025-01-01'),
		('TKNV0010', 'cuong_do', 'NV0010', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-08'),
		('TKNV0011', 'mai_le', 'NV0011', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-08'),
		('TKNV0012', 'binh_nguyen', 'NV0012', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-08'),
		('TKNV0013', 'thao_tran', 'NV0013', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-08'),
		('TKNV0014', 'duy_pham', 'NV0014', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-08'),
		('TKNV0015', 'ngoc_hoang', 'NV0015', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-08'),
		('TKNV0016', 'an_vu', 'NV0016', 'Nnk1234@', N'Nhân viên kho', 'Avatar.png', 1, '2025-01-08'),
		('TKNV0017', 'lan_bui', 'NV0017', 'Qlch123@', N'Quản lý cửa hàng', 'Avatar.png', 1, '2025-01-08'),
		('TKNV0018', 'hung_ngo', 'NV0018', 'Qlk1234@', N'Quản lý kho', 'Avatar.png', 1, '2025-01-08'),
		('TKNV0019', 'tam_do', 'NV0019', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-08'),
		('TKNV0020', 'hang_dang', 'NV0020', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-15'),
		('TKNV0021', 'phuc_nguyen', 'NV0021', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-15'),
		('TKNV0022', 'ngoc_le', 'NV0022', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-15'),
		('TKNV0023', 'khoa_tran', 'NV0023', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-15'),
		('TKNV0024', 'ha_pham', 'NV0024', 'Nnk1234@', N'Nhân viên kho', 'Avatar.png', 1, '2025-01-15'),
		('TKNV0025', 'cuong_vu', 'NV0025', 'Qlch123@', N'Quản lý cửa hàng', 'Avatar.png', 1, '2025-01-15'),
		('TKNV0026', 'tuyet_hoang', 'NV0026', 'Qlk1234@', N'Quản lý kho', 'Avatar.png', 1, '2025-01-15'),
		('TKNV0027', 'son_bui', 'NV0027', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-15'),
		('TKNV0028', 'linh_ngo', 'NV0028', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-15'),
		('TKNV0029', 'trung_do', 'NV0029', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-22'),
		('TKNV0030', 'minh_dang', 'NV0030', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-22'),
		('TKNV0031', 'hung_nguyen', 'NV0031', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-22'),
		('TKNV0032', 'tam_le', 'NV0032', 'Nnk1234@', N'Nhân viên kho', 'Avatar.png', 1, '2025-01-22'),
		('TKNV0033', 'dung_tran', 'NV0033', 'Qlch123@', N'Quản lý cửa hàng', 'Avatar.png', 1, '2025-01-22'),
		('TKNV0034', 'yen_pham', 'NV0034', 'Qlk1234@', N'Quản lý kho', 'Avatar.png', 1, '2025-01-22'),
		('TKNV0035', 'dat_vu', 'NV0035', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-22'),
		('TKNV0036', 'kim_hoang', 'NV0036', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-22'),
		('TKNV0037', 'hao_bui', 'NV0037', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-22'),
		('TKNV0038', 'lan_ngo', 'NV0038', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-22'),
		('TKNV0039', 'tu_do', 'NV0039', 'Nvbh123@', N'Nhân viên bán hàng', 'Avatar.png', 1, '2025-01-29'),
		('TKNV0040', 'thuy_dang', 'NV0040', 'nhk123', N'Nhân viên kho', 'Avatar.png', 1, '2025-01-29'), 
		('TKNV0041', 'hong_nguyen', 'NV0041', 'Qlch123@', N'Quản lý cửa hàng', 'Avatar.png', 1, '2025-01-29'),
		('TKNV0042', 'hai_le', 'NV0042', 'Qlk1234@', N'Quản lý kho', 'Avatar.png', 1, '2025-01-29'),
		('TKNV0043', 'le_tran', 'NV0043', 'Qtv1234@', N'Quản trị viên', 'Avatar.png', 1, '2025-01-29'),
		('TKNV0044', 'nam_pham', 'NV0044', 'Qtv1234@', N'Quản trị viên', 'Avatar.png', 1, '2025-01-29'),
		('TKNV0045', 'bao_vu', 'NV0045', 'Qtv1234@', N'Quản trị viên', 'Avatar.png', 1, '2025-01-29'),
		('TKNV0046', 'vinh_hoang', 'NV0046', 'Qlch123@', N'Quản lý cửa hàng', 'Avatar.png', 1, '2025-01-29'),
		('TKNV0047', 'thu_bui', 'NV0047', 'Nnk1234@', N'Nhân viên kho', 'Avatar.png', 1, '2025-01-29'),
		('TKNV0048', 'tin_ngo', 'NV0048', 'Nnk1234@', N'Nhân viên kho', 'Avatar.png', 1, '2025-01-29'),
		('TKNV0049', 'loan_do', 'NV0049', 'Nnk1234@', N'Nhân viên kho', 'Avatar.png', 1, '2025-01-29'),
		('TKNV0050', 'thao_dang', 'NV0050', 'Qlk1234@', N'Quản lý kho', 'Avatar.png', 1, '2025-01-29');

---------------------------------------------------------------------
-- TRIGGER BẢNG TÀI KHOẢN NHÂN VIÊN
---------------------------------------------------------------------
CREATE TRIGGER trg_NhapTaiKhoanNV_Validate
ON tblTaiKhoanNV
INSTEAD OF INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NewCode NCHAR(8);

    -----------------------------------------------------------------
    -- 1. TỰ SINH MÃ TÀI KHOẢN NHÂN VIÊN (INSERT)
    -----------------------------------------------------------------
    IF EXISTS (SELECT 1 FROM inserted)
       AND NOT EXISTS (SELECT 1 FROM deleted)
    BEGIN
        DECLARE @LastCode NCHAR(8), @Num INT;

        SELECT @LastCode = MAX(MaTaiKhoanNV) FROM tblTaiKhoanNV;

        IF @LastCode IS NULL 
            SET @NewCode = 'TKNV0001';
        ELSE
        BEGIN
            SET @Num = CAST(SUBSTRING(@LastCode, 5, 4) AS INT) + 1;
            SET @NewCode = 'TKNV' + RIGHT('0000' + CAST(@Num AS VARCHAR(4)), 4);
        END
    END

    -----------------------------------------------------------------
    -- 2. KIỂM TRA TÊN TÀI KHOẢN
    -----------------------------------------------------------------
    -- Chỉ chữ, số, _, ., -
    IF EXISTS (
        SELECT 1 FROM inserted
        WHERE TenTaiKhoan LIKE '%[^a-zA-Z0-9_.-]%'
    )
    BEGIN
        RAISERROR(N'Tên tài khoản chỉ được chứa chữ, số, dấu _ . -', 16, 1);
        RETURN;
    END

    -- Không trùng tên tài khoản
    IF EXISTS (
        SELECT 1
        FROM inserted i
        WHERE EXISTS (
            SELECT 1 FROM tblTaiKhoanNV t
            WHERE t.TenTaiKhoan = i.TenTaiKhoan
              AND t.MaTaiKhoanNV <> i.MaTaiKhoanNV
        )
    )
    BEGIN
        RAISERROR(N'Tên tài khoản đã tồn tại.', 16, 1);
        RETURN;
    END

    -----------------------------------------------------------------
    -- 3. KIỂM TRA MẬT KHẨU
    -----------------------------------------------------------------
    IF EXISTS (
        SELECT 1 FROM inserted
        WHERE LEN(MatKhau) < 8
           OR LEN(MatKhau) > 50
           OR MatKhau LIKE ' %'
           OR MatKhau LIKE '% '
           OR MatKhau NOT LIKE '%[a-z]%'      -- phải có chữ thường
           OR MatKhau NOT LIKE '%[A-Z]%'      -- phải có chữ hoa
           OR MatKhau NOT LIKE '%[0-9]%'      -- phải có số
           OR MatKhau NOT LIKE '%[@\-\_\!\#\$\%\&\*]%'   -- ký tự đặc biệt
           OR MatKhau LIKE '% %'
           OR MatKhau LIKE '%aaaa%'
           OR MatKhau LIKE '%1111%'
           OR MatKhau LIKE '%123456%'
    )
    BEGIN
        RAISERROR(N'Mật khẩu không hợp lệ, vui lòng kiểm tra lại.', 16, 1);
        RETURN;
    END

    -----------------------------------------------------------------
    -- 4. INSERT / UPDATE DỮ LIỆU
    -----------------------------------------------------------------
    IF NOT EXISTS (SELECT * FROM deleted)   -- INSERT
    BEGIN
        INSERT INTO tblTaiKhoanNV
            (MaTaiKhoanNV, TenTaiKhoan, MaNhanVien,
             MatKhau, Quyen, HinhAnh, TrangThaiTaiKhoan, NgayCapNhat)
        SELECT 
            @NewCode,
            TenTaiKhoan,
            MaNhanVien,
            MatKhau,
            Quyen,                 -- Nhân viên có nhiều quyền khác nhau → giữ nguyên
            HinhAnh,
            TrangThaiTaiKhoan,
            GETDATE()
        FROM inserted;
    END
    ELSE  -- UPDATE
    BEGIN
        UPDATE TK
        SET 
            TenTaiKhoan = i.TenTaiKhoan,
            MaNhanVien = i.MaNhanVien,
            MatKhau = i.MatKhau,
            Quyen = i.Quyen,
            HinhAnh = i.HinhAnh,
            TrangThaiTaiKhoan = i.TrangThaiTaiKhoan,
            NgayCapNhat = GETDATE()
        FROM tblTaiKhoanNV TK
        INNER JOIN inserted i ON TK.MaTaiKhoanNV = i.MaTaiKhoanNV;
    END
END
GO



									--------------------------------------------------------------------
															-- BẢNG KHÁCH HÀNG --
                                    --------------------------------------------------------------------
-- Dữ liệu Khách hàng 
insert into tblKhachHang (MaKhachHang, HoKhachHang, TenKhachHang, LoaiKhachHang, NgaySinh, GioiTinh, DienThoai, DiaChi, Email, NgayDangKy, MaNhanVien, NgayCapNhat)
values	('KH0001', N'Nguyễn', N'Văn An', N'Thành viên', '1995-03-15', N'Nam', '0987654321', N'27 Đường Ngô Đức Kế, Phường Sài Gòn, TP.HCM', 'an.nguyen@gmail.com', '2025-04-11', 'NV0002', '2025-04-11'),
		('KH0002', N'Trần', N'Hoàng Bình', N'Thành viên', '1988-07-22', N'Nam', '0978123456', N'91 Đường Nguyễn Hữu Cảnh, Phường Thạnh Mỹ Tây, TP.HCM', 'binh.tran@gmail.com', '2025-04-11', 'NV0002', '2025-04-11'),
		('KH0003', N'Lê', N'Thị Cúc', N'Thành viên', '2000-11-09', N'Nữ', '0912345678', N'331 Đường An Dương Vương, Phường Chợ Quán, TP.HCM', 'cuc.le@gmail.com', '2025-04-11', 'NV0002', '2025-04-11'),
		('KH0004', N'Phạm', N'Hữu Duy', N'Thành viên', '1993-05-30', N'Nam', '0934567890', N'491/2 Đường Lê Văn Sỹ, Phường Nhiêu Lộc, TP.HCM', 'duy.pham@gmail.com', '2025-04-11', 'NV0002', '2025-04-11'),
		('KH0005', N'Hoàng', N'Ngọc Em', N'Thành viên', '1997-09-12', N'Nữ', '0909876543', N'7 Đường Phan Văn Trị, Phường Gò Vấp, TP.HCM', 'em.hoang@gmail.com', '2025-04-11', 'NV0002', '2025-04-11'),
		('KH0006', null, null, N'Vãng lai', null, null, null, null, null, null, 'NV0002', '2025-04-11'), 
		('KH0007', N'Vũ', N'Thanh Giang', N'Thành viên', '1992-06-18', N'Nữ', '0956781234', N'237 Đường Bến Vân Đồn, Phường Vĩnh Hội, TP.HCM', 'giang.vu@gmail.com', '2025-04-12', 'NV0002', '2025-04-12'),
		('KH0008', N'Bùi', N'Đức Hào', N'Thành viên', '1999-08-21', N'Nam', '0971234567', N'18C Đường Phan Văn Trị, Phường Gò Vấp, TP.HCM', 'hao.bui@gmail.com', '2025-04-12', 'NV0002', '2025-04-12'),
		('KH0009', N'Ngô', N'Bảo Hiền', N'Thành viên', '1990-04-10', N'Nữ', '0945678901', N'1 Đường Số 7, Phường Khánh Hội, TP.HCM', 'hien.ngo@gmail.com', '2025-04-12', 'NV0036', '2025-04-12'),
		('KH0010', N'Đặng', N'Gia Khánh', N'Thành viên', '1996-02-28', N'Nam', '0916782345', N'262 Đường Ung Văn Khiêm, Phường Thạnh Mỹ Tây, TP.HCM', 'khanh.dang@gmail.com', '2025-04-12', 'NV0003', '2025-04-12'),
		('KH0011', N'Nguyễn', N'Thanh Linh', N'Thành viên', '1998-11-15', N'Nữ', '0901234567', N'68 Đường Trần Bình Trọng, Phường Chợ Quán, TP.HCM', 'linh.nguyen@gmail.com', '2025-04-12', 'NV0003', '2025-04-12'), --SN
		('KH0012', N'Trần', N'Việt Minh', N'Thành viên', '1991-07-30', N'Nam', '0987456123', N'469 Đường Điện Biên Phủ, Phường Thạnh Mỹ Tây, TP.HCM', 'minh.tran@gmail.com', '2025-04-12', 'NV0003', '2025-04-12'),
		('KH0013', N'Lê', N'Thị Nga', N'Thành viên', '1994-09-19', N'Nữ', '0912233445', N'193A/D3 Đường Nam Kỳ Khởi Nghĩa, Phường Xuân Hòa, TP.HCM', 'nga.le@gmail.com', '2025-04-12', 'NV0003', '2025-04-12'),
		('KH0014', N'Phạm', N'Thanh Oanh', N'Thành viên', '1989-05-05', N'Nữ', '0976543210', N'496/38 Đường Dương Quảng Hàm, Phường An Nhon, TP.HCM', 'oanh.pham@gmail.com', '2025-04-12', 'NV0003', '2025-04-12'),
		('KH0015', N'Hoàng', N'Hữu Phát', N'Thành viên', '2001-03-08', N'Nam', '0938765432', N'46 Đường Trịnh Văn Cấn, Phường Bến Thành, TP.HCM', 'phat.hoang@gmail.com', '2025-04-12', 'NV0036', '2025-04-12'),
		('KH0016', null, null, N'Vãng lai', null, null, null, null, null, null, 'NV0036', '2025-04-15'),
		('KH0017', N'Vũ', N'Trọng Sơn', N'Thành viên', '1993-06-14', N'Nam', '0943216789', N'66 Đường Tôn Thất Thuyết, Phường Xóm Chiếu, TP.HCM', 'son.vu@gmail.com', '2025-04-15', 'NV0036', '2025-04-15'),
		('KH0018', N'Bùi', N'Anh Tuấn', N'Thành viên', '1986-08-29', N'Nam', '0923456781', N'10 Đường Nguyễn Tri Phương, Phường An Đông, TP.HCM', 'tuan.bui@gmail.com', '2025-04-15', 'NV0036', '2025-04-15'),
		('KH0019', N'Ngô', N'Kim Uyên', N'Thành viên', '1999-02-07', N'Nữ', '0919876543', N'11 Đường Phan Văn Trị, Phường An Nhon, TP.HCM', 'uyen.ngo@gmail.com', '2025-04-15', 'NV0036', '2025-04-15'),
		('KH0020', N'Đặng', N'Ngọc Vân', N'Thành viên', '1992-10-10', N'Nam', '0905647382', N'11 Đường Thảo Điền, Phường An Khánh, TP.HCM', 'van.dang@gmail.com', '2025-04-15', 'NV0020', '2025-04-15'),
		('KH0021', N'Lý', N'Thanh Hoa', N'Thành viên', '1990-04-05', N'Nữ', '0932214567', N'19 Đường Lê Thánh Tôn, Phường Sài Gòn, TP.HCM', 'hoa.ly@gmail.com', '2025-04-16', 'NV0020', '2025-04-16'),
		('KH0022', null, null, N'Vãng lai', null, null, null, null, null, null, 'NV0020', '2025-04-16'),
		('KH0023', N'Phan', N'Thị Thúy', N'Thành viên', '1987-09-17', N'Nữ', '0974523789', N'230 Đường Hồng Bàng, Phường Chợ Lớn, TP.HCM', 'thuy.phan@gmail.com', '2025-04-16', 'NV0020', '2025-04-16'),
		('KH0024', N'Ngô', N'Minh Tâm', N'Thành viên', '1993-12-01', N'Nam', '0913456782', N'14A Bis Đường Đặng Văn Ngữ, Phường Phú Nhuận, TP.HCM', 'tam.ngo@gmail.com', '2025-04-16', 'NV0020', '2025-04-16'),--SN
		('KH0025', N'Đoàn', N'Duy Phúc', N'Thành viên', '1988-02-10', N'Nam', '0945876352', N'3/5 Đường Hoàng Sa, Phường Sài Gòn, TP.HCM', 'phuc.doan@gmail.com', '2025-04-16', 'NV0020', '2025-04-16'),
		('KH0026', N'Lâm', N'Ngọc Yến', N'Thành viên', '2000-07-19', N'Nữ', '0921456789', N'39 Nguyễn Hữu Huân, Phường Hoàn Kiếm, Hà Nội', 'yen.lam@gmail.com', '2025-04-16', 'NV0012', '2025-04-16'),
		('KH0027', N'Tạ', N'Hữu Hiếu', N'Thành viên', '1991-08-31', N'Nam', '0909873124', N'34 Bạch Đằng, Phường Hải Châu, Đà Nẵng', 'hieu.ta@gmail.com', '2025-04-16', 'NV0021', '2025-04-16'),
		('KH0028', N'Quách', N'Gia Bảo', N'Thành viên', '1996-11-14', N'Nam', '0967895432', N'Số 1 Ngách 53, Ngõ 252 Tây Sơn, Phường Đống Đa, Hà Nội', 'bao.quach@gmail.com', '2025-04-16', 'NV0012', '2025-04-16'), --SN 
		('KH0029', N'Cao', N'Anh Dũng', N'Thành viên', '1994-05-22', N'Nam', '0956782341', N'72 Hàm Nghi, Phường Thanh Khê, Đà Nẵng', 'dung.cao@gmail.com', '2025-04-16', 'NV0021', '2025-04-16'),
		('KH0030', N'Dương', N'Thị Lan', N'Thành viên', '1985-09-29', N'Nữ', '0935462718', N'104 Quảng An, Phường Tây Hồ, Hà Nội', 'lan.duong@gmail.com', '2025-04-18', 'NV0012', '2025-04-18'), 
		('KH0031', N'Trịnh', N'Quốc Huy', N'Thành viên', '1992-12-15', N'Nam', '0923451234', N'05 An Thượng 2, Phường Ngũ Hành Sơn, Đà Nẵng', 'huy.trinh@gmail.com', '2025-04-18', 'NV0021', '2025-04-18'),
		('KH0032', N'Ninh', N'Thùy Dương', N'Thành viên', '1998-01-05', N'Nữ', '0912346789', N'106 Nguyễn Văn Cừ, Phường Long Biên, Hà Nội', 'duong.ninh@gmail.com', '2025-04-18', 'NV0012', '2025-04-18'), 
		('KH0033', N'Kim', N'Hữu Tình', N'Thành viên', '1993-04-11', N'Nam', '0985123476', N'13 Đinh Tiên Hoàng, Phường Hoàn Kiếm, Hà Nội', 'tinh.kim@gmail.com', '2025-04-22', 'NV0012', '2025-04-22'), --SN 
		('KH0034', N'Ngô', N'Phương Nhi', N'Thành viên', '1997-06-19', N'Nữ', '0976548971', N'214 Lacasta, Phường Hà Đông, Hà Nội', 'nhi.ngo@gmail.com', '2025-04-22', 'NV0029', '2025-04-22'),
		('KH0035', N'Dương', N'Thanh Tuấn', N'Thành viên', '1995-08-25', N'Nam', '0963218745', N'8 Chân Cầm, Phường Hoàn Kiếm, Hà Nội', 'tuan.duong@gmail.com', '2025-04-22', 'NV0029', '2025-04-22'),
		('KH0036', N'Lã', N'Ngọc Anh', N'Thành viên', '1992-10-07', N'Nữ', '0954876321', N'11 Hàng Gai, Phường Hoàn Kiếm, Hà Nội', 'anh.la@gmail.com', '2025-04-22', 'NV0029', '2025-04-22'), 
		('KH0037', N'Mạc', N'Quang Huy', N'Thành viên', '1994-12-12', N'Nam', '0941237895', N'96 Trần Phú, Phường Hải Châu, Đà Nẵng', 'huy.mac@gmail.com', '2025-04-23', 'NV0021', '2025-04-23'),--SN
		('KH0038', N'Tiêu', N'Gia Hân', N'Thành viên', '1998-01-30', N'Nữ', '0936584127', N'59 Hải Phòng, Phường Hải Châu, Đà Nẵng', 'han.tieu@gmail.com', '2025-04-23', 'NV0021', '2025-04-23'),
		('KH0039', N'Triệu', N'Việt Hoàng', N'Thành viên', '1990-03-22', N'Nam', '0923146785', N'142/3 Lê Độ, Phường Thanh Khê, Đà Nẵng', 'hoang.trieu@gmail.com', '2025-04-23', 'NV0029', '2025-04-23'),
		('KH0040', N'Cung', N'Minh Châu', N'Thành viên', '1996-05-15', N'Nữ', '0912365478', N'1 Đường Hồ Xanh, Phường Sơn Trà, Đà Nẵng', 'chau.cung@gmail.com', '2025-04-27', 'NV0029', '2025-04-27'),
		('KH0041', N'Thạch', N'Trung Kiên', N'Thành viên', '1993-03-14', N'Nam', '0901123456', N'115 Ngô Thì Sĩ, Phường Ngũ Hành Sơn, Đà Nẵng', 'kien.thach@gmail.com', '2025-04-27', 'NV0029', '2025-04-27'),
		('KH0042', N'Tống', N'Thị Thảo', N'Thành viên', '1996-08-17', N'Nữ', '0912234567', N'92/17 Đường Phạm Ngọc Thạch, Phường Xuân Hòa, TP.HCM', 'thao.tong@gmail.com', '2025-04-27', 'NV0020', '2025-04-27'),
		('KH0043', N'Vi', N'Văn Toàn', N'Thành viên', '1992-04-25', N'Nam', '0923345678', N'10B Đường Tôn Đức Thắng, Phường Sài Gòn, TP.HCM', 'toan.vi@gmail.com', '2025-04-27', 'NV0011', '2025-04-27'),
		('KH0044', N'Lữ', N'Thị Bích', N'Thành viên', '1990-12-30', N'Nữ', '0934456789', N'103Bis Đường Võ Thị Sáu, Phường Xuân Hòa, TP.HCM', 'bich.lu@gmail.com', '2025-04-27', 'NV0011', '2025-04-27'), --SN
		('KH0045', N'Lâm', N'Anh Thư', N'Thành viên', '1995-11-19', N'Nữ', '0945567890', N'5 Đường Trần Quý Khoách, Phường Tân Định, TP.HCM', 'thu.lam@gmail.com', '2025-04-27', 'NV0037', '2025-04-27'), --SN
		('KH0046', N'Từ', N'Hồng Phúc', N'Thành viên', '1989-06-09', N'Nam', '0956678901', N'28 Đường Thảo Điền, Phường An Khánh, TP.HCM', 'phuc.tu@gmail.com', '2025-04-30', 'NV0037', '2025-04-30'),
		('KH0047', N'Huỳnh', N'Phan Hòa', N'Thành viên', '1991-05-28', N'Nữ', '0967789012', N'212 Đường Lê Lai, Phường Bến Thành, TP.HCM', 'hoa.huynh@gmail.com', '2025-04-30', 'NV0037', '2025-04-30'),
		('KH0048', N'Hà', N'Minh Khôi', N'Thành viên', '1997-02-03', N'Nam', '0978890123', N'25A Đường Phạm Viết Chánh, Phường Cầu Ông Lãnh, TP.HCM', 'khoi.ha@gmail.com', '2025-04-30', 'NV0037', '2025-04-30'),
		('KH0049', N'Lương', N'Thị Hạ', N'Thành viên', '1994-09-15', N'Nữ', '0989901234', N'42 Đường Xuân Thủy, Phường An Khánh, TP.HCM', 'ha.luong@gmail.com', '2025-04-30', 'NV0028', '2025-04-30'),
		('KH0050', N'Triệu', N'Việt Dũng', N'Thành viên', '1990-01-07', N'Nam', '0911012345', N'1A Đường Phan Tôn, Phường Tân Định, TP.HCM', 'dung.trieu@gmail.com', '2025-04-30', 'NV0011', '2025-04-30'),
		('KH0051', N'Lý', N'Thị Mai', N'Thành viên', '1996-10-01', N'Nữ', '0922123456', N'11 Đường Nguyễn Oanh, Phường Gò Vấp, TP.HCM', 'mai.ly@gmail.com', '2025-04-30', 'NV0011', '2025-04-30'),
		('KH0052', null, null, N'Vãng lai', null, null, null, null, null, null, 'NV0011', '2025-04-30'),
		('KH0053', N'Đặng', N'Mỹ Linh', N'Thành viên', '1993-03-09', N'Nữ', '0944345678', N'62 Đường Nguyễn Thị Thập, Phường Tân Thuận, TP.HCM', 'linh.dang@gmail.com', '2025-05-01', 'NV0011', '2025-05-01'),
		('KH0054', N'Bạch', N'Ngọc Sơn', N'Thành viên', '1988-07-01', N'Nam', '0955456789', N'326 Đường Lê Văn Lương, Phường Tân Hưng, TP.HCM,', 'son.bach@gmail.com', '2025-05-01', 'NV0011', '2025-05-01'),
		('KH0055', N'Trịnh', N'Thảo Vy', N'Thành viên', '1999-01-25', N'Nữ', '0966567890', N'11 Đường Hoa Cau, Phường Cầu Kiệu, TP.HCM', 'vy.trinh@gmail.com', '2025-05-01', 'NV0011', '2025-05-01'),
		('KH0056', N'Tô', N'Nhật Nam', N'Thành viên', '1992-02-20', N'Nam', '0977678901', N'146 Đường Lương Ngọc Quyến, Phường An Nhơn, TP.HCM,', 'nam.to@gmail.com', '2025-05-01', 'NV0028', '2025-05-01'),
		('KH0057', N'Tạ', N'Thị Kim', N'Thành viên', '1990-06-13', N'Nữ', '0988789012', N'264 Đường Lê Văn Quới, Phường Bình Trị Đông, TP.HCM', 'kim.ta@gmail.com', '2025-05-01', 'NV0028', '2025-05-01'),
		('KH0058',null, null, N'Vãng lai', null, null, null, null, null, null, 'NV0028', '2025-05-01'),
		('KH0059', N'Nguyễn', N'Mai Trang', N'Thành viên','1994-12-17', N'Nữ', '0911123456', N'8 Đường Nguyễn Ư Dĩ, Phường An Khánh, TP.HCM', 'trang.nguyen@gmail.com', '2025-05-02', 'NV0028', '2025-05-02'), --SN
		('KH0060', N'Lê', N'Tấn Phát', N'Thành viên', '1993-07-08', N'Nam', '0922234567', N'20 Đường Hoa Sữa, Phường Cầu Kiệu, TP.HCM', 'phat.le@gmail.com', '2025-05-02', 'NV0028', '2025-05-02');


--Trigger 
CREATE TRIGGER trg_KhachHang_VangLai_ResetHang
ON tblKhachHang
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Ngăn trigger tự kích hoạt lại khi chính nó update
    IF TRIGGER_NESTLEVEL() > 1 RETURN;

    -- Cập nhật cho khách hàng loại 'Vãng lai'
    UPDATE kh
    SET HangThanhVien = NULL
    FROM tblKhachHang kh
    JOIN inserted i ON kh.MaKhachHang = i.MaKhachHang
    WHERE i.LoaiKhachHang = N'Vãng lai';
END
GO


CREATE TRIGGER trg_XoaKhachHang
ON tblKhachHang
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Xóa dữ liệu trong bảng tblMaKMCuaKH (FK đến khách hàng)
    DELETE FROM tblMaKMCuaKH
    WHERE MaKhachHang IN (SELECT MaKhachHang FROM deleted);

    -- Nếu sau này bà còn có các bảng FK khác đến khách hàng (ví dụ lịch sử mua hàng, điểm,...)
    -- thì thêm vào đây theo đúng thứ tự:
    --
    -- DELETE FROM TenBangPhu
    -- WHERE MaKhachHang IN (SELECT MaKhachHang FROM deleted);

    -- 2. Cuối cùng xóa khách hàng
    DELETE FROM tblKhachHang
    WHERE MaKhachHang IN (SELECT MaKhachHang FROM deleted);
END
GO


-----------------------------------------------------------------
-- TRIGGER KHÁCH HÀNG 
-----------------------------------------------------------------
CREATE OR ALTER TRIGGER trg_NhapKhachHang_Validate
ON tblKhachHang
INSTEAD OF INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

        -- Kiểm tra Vãng lai
    IF EXISTS (
        SELECT 1
        FROM inserted
        WHERE LoaiKhachHang = N'Vãng lai'
          AND (
                NULLIF(LTRIM(RTRIM(HoKhachHang)),'') IS NOT NULL
             OR NULLIF(LTRIM(RTRIM(TenKhachHang)),'') IS NOT NULL
             OR NgaySinh IS NOT NULL
             OR NULLIF(LTRIM(RTRIM(DienThoai)),'') IS NOT NULL
             OR NULLIF(LTRIM(RTRIM(DiaChi)),'') IS NOT NULL
             OR NULLIF(LTRIM(RTRIM(Email)),'') IS NOT NULL
          )
    )
    BEGIN
        RAISERROR(N'Khách hàng Vãng lai không được nhập thông tin cá nhân.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END;


    -----------------------------------------------------------------
    -- 2. KIỂM TRA HỌ KHÁCH HÀNG (chỉ khi KH không phải Vãng lai)
    -----------------------------------------------------------------
    IF EXISTS (
        SELECT 1 FROM inserted
        WHERE LoaiKhachHang <> N'Vãng lai'
          AND (HoKhachHang LIKE '%[^a-zA-Z ]%'
               OR HoKhachHang <> STUFF(LOWER(HoKhachHang), 1, 1, UPPER(LEFT(HoKhachHang,1))))
    )
    BEGIN
        RAISERROR(N'Họ khách hàng chỉ được chứa chữ và khoảng trắng, viết hoa chữ cái đầu.', 16, 1);
        RETURN;
    END;

    -----------------------------------------------------------------
    -- 3. KIỂM TRA TÊN KHÁCH HÀNG (chỉ khi KH không phải Vãng lai)
    -----------------------------------------------------------------
    IF EXISTS (
        SELECT 1 FROM inserted
        WHERE LoaiKhachHang <> N'Vãng lai'
          AND (TenKhachHang LIKE '%[^a-zA-Z ]%'
               OR TenKhachHang <> STUFF(LOWER(TenKhachHang), 1, 1, UPPER(LEFT(TenKhachHang,1))))
    )
    BEGIN
        RAISERROR(N'Tên khách hàng chỉ được chứa chữ và khoảng trắng, viết hoa chữ cái đầu.', 16, 1);
        RETURN;
    END;

    -----------------------------------------------------------------
    -- 4. KIỂM TRA NGÀY SINH (chỉ khi KH không phải Vãng lai)
    -----------------------------------------------------------------
    IF EXISTS (
        SELECT 1 FROM inserted
        WHERE LoaiKhachHang <> N'Vãng lai'
          AND (NgaySinh IS NULL OR DATEDIFF(YEAR, NgaySinh, GETDATE()) < 18)
    )
    BEGIN
        RAISERROR(N'Khách hàng phải trên 18 tuổi.', 16, 1);
        RETURN;
    END;

    -----------------------------------------------------------------
    -- 5. KIỂM TRA SỐ ĐIỆN THOẠI (chỉ khi KH không phải Vãng lai)
    -----------------------------------------------------------------
    IF EXISTS (
        SELECT 1 FROM inserted
        WHERE LoaiKhachHang <> N'Vãng lai'
          AND (LEN(DienThoai) <> 10
               OR DienThoai LIKE '%[^0-9]%'
               OR LEFT(DienThoai, 3) NOT IN ('032','033','034','035','036','037','038','039',
                                             '050','051','052','053','054','055','056','057','058','059',
                                             '070','071','072','073','074','075','076','077','078','079',
                                             '080','081','082','083','084','085','086','087','088','089',
                                             '090','091','092','093','094','095','096','097','098','099'))
    )
    BEGIN
        RAISERROR(N'Số điện thoại phải gồm 10 chữ số và bắt đầu bằng 03x, 05x, 07x, 08x, 09x.', 16, 1);
        RETURN;
    END;

    -----------------------------------------------------------------
    -- 6. KIỂM TRA EMAIL (chỉ khi KH không phải Vãng lai)
    -----------------------------------------------------------------
    IF EXISTS (
        SELECT 1
        FROM inserted
        CROSS APPLY (
            SELECT 
                Email AS FullEmail,
                LEFT(Email, CHARINDEX('@', Email)-1) AS LocalPart,
                SUBSTRING(Email, CHARINDEX('@', Email)+1, LEN(Email)) AS Domain
        ) AS EmailParts
        WHERE LoaiKhachHang <> N'Vãng lai'
          AND (
                FullEmail IS NULL
                OR LEN(FullEmail) < 6
                OR LEN(FullEmail) > 254
                OR FullEmail LIKE '% %'
                OR LEFT(FullEmail,1) IN ('.','-')
                OR RIGHT(FullEmail,1) IN ('.','-')
                OR LEN(FullEmail) - LEN(REPLACE(FullEmail,'@','')) <> 1
                OR FullEmail NOT LIKE '%@%.%'
                OR CHARINDEX('..', LocalPart) > 0
                OR FullEmail LIKE '%.@%'
                OR FullEmail LIKE '%@.%'
                OR LocalPart LIKE '.%'
                OR CHARINDEX('.', Domain) = 0
                OR Domain LIKE '%[^A-Za-z0-9.-]%'
                OR Domain LIKE '-%' OR Domain LIKE '%-'
          )
    )
    BEGIN
        RAISERROR(N'Email không hợp lệ.', 16, 1);
        RETURN;
    END;

    -----------------------------------------------------------------
    -- 7. KIỂM TRA ĐỊA CHỈ 
    -----------------------------------------------------------------

    -- 7.1 Không cho phép ký tự nguy hiểm
    IF EXISTS (
        SELECT 1
        FROM inserted
        WHERE DiaChi IS NOT NULL
          AND DiaChi LIKE '%[<>$%^&*{}[\]|]%'
    )
    BEGIN
        RAISERROR (
            N'Địa chỉ chứa ký tự không hợp lệ.',
            16, 1
        );
        RETURN;
    END;

    -- 7.2 Phải bắt đầu bằng chữ hoặc số (Unicode OK)
    IF EXISTS (
        SELECT 1
        FROM inserted
        WHERE DiaChi IS NOT NULL
          AND LEFT(LTRIM(DiaChi),1) NOT LIKE '[0-9A-Za-zÀ-ỹ]'
    )
    BEGIN
        RAISERROR (
            N'Địa chỉ phải bắt đầu bằng chữ hoặc số.',
            16, 1
        );
        RETURN;
    END;

-- 7.3 Cấm "/" đứng đầu hoặc đứng cuối (VD: /12 hoặc 12/)
-- Cho phép: 12/3, A/12, 193A/D3

IF EXISTS (
    SELECT 1
    FROM inserted
    WHERE DiaChi IS NOT NULL
      AND CHARINDEX('/', DiaChi) > 0
      AND (
            DiaChi LIKE '% /%'    -- trước "/" là khoảng trắng
         OR DiaChi LIKE '%/ %'    -- sau "/" là khoảng trắng
         OR DiaChi LIKE '/%'      -- "/" ở đầu chuỗi
         OR DiaChi LIKE '%/'      -- "/" ở cuối chuỗi
      )
)
BEGIN
    RAISERROR (
        N'Địa chỉ không hợp lệ: dấu "/" không được đứng đầu hoặc đứng cuối.',
        16, 1
    );
    RETURN;
END;

    -----------------------------------------------------------------
    -- 8. INSERT / UPDATE VÀ GHI NHẬT KÝ NGÀY CẬP NHẬT
    -----------------------------------------------------------------
        IF NOT EXISTS (SELECT * FROM deleted)
    BEGIN
        INSERT INTO tblKhachHang 
            (MaKhachHang, HoKhachHang, TenKhachHang, LoaiKhachHang, NgaySinh,
             GioiTinh, DienThoai, DiaChi, Email, NgayDangKy, DiemTichLuy, 
             HangThanhVien, MaNhanVien, NgayCapNhat)
        SELECT 
            MaKhachHang,
            HoKhachHang,
            TenKhachHang,
            LoaiKhachHang,
            NgaySinh,
            GioiTinh,
            DienThoai,
            DiaChi,
            Email,
            NgayDangKy, 
            DiemTichLuy, 
            CASE 
                WHEN LoaiKhachHang = N'Thành viên' THEN N'Watsons Club'
                ELSE HangThanhVien
            END,
            MaNhanVien, 
            GETDATE()
        FROM inserted
        WHERE NOT (
              LoaiKhachHang = N'Vãng lai'
              AND (
                    ISNULL(LTRIM(RTRIM(HoKhachHang)),'') <> ''
                 OR ISNULL(LTRIM(RTRIM(TenKhachHang)),'') <> ''
                 OR NgaySinh IS NOT NULL
                 OR ISNULL(LTRIM(RTRIM(DienThoai)),'') <> ''
                 OR ISNULL(LTRIM(RTRIM(DiaChi)),'') <> ''
                 OR ISNULL(LTRIM(RTRIM(Email)),'') <> ''
              )
        );
    END
    ELSE
    BEGIN
        UPDATE KH
        SET 
            HoKhachHang = i.HoKhachHang,
            TenKhachHang = i.TenKhachHang,
            LoaiKhachHang = i.LoaiKhachHang,
            NgaySinh = i.NgaySinh,
            GioiTinh = i.GioiTinh,
            DienThoai = i.DienThoai,
            DiaChi = i.DiaChi,
            Email = i.Email,
            NgayDangKy = i.NgayDangKy,
            DiemTichLuy = i.DiemTichLuy,
            HangThanhVien = CASE 
                                WHEN i.LoaiKhachHang = N'Thành viên' THEN N'Watsons Club'
                                WHEN i.LoaiKhachHang = N'Vãng lai' THEN NULL
                                ELSE i.HangThanhVien
                            END,
            MaNhanVien = i.MaNhanVien,
            NgayCapNhat = GETDATE()
        FROM tblKhachHang KH
        INNER JOIN inserted i ON KH.MaKhachHang = i.MaKhachHang
        WHERE NOT (
              i.LoaiKhachHang = N'Vãng lai'
              AND (
                    ISNULL(LTRIM(RTRIM(i.HoKhachHang)),'') <> ''
                 OR ISNULL(LTRIM(RTRIM(i.TenKhachHang)),'') <> ''
                 OR i.NgaySinh IS NOT NULL
                 OR ISNULL(LTRIM(RTRIM(i.DienThoai)),'') <> ''
                 OR ISNULL(LTRIM(RTRIM(i.DiaChi)),'') <> ''
                 OR ISNULL(LTRIM(RTRIM(i.Email)),'') <> ''
              )
        );
    END;
END
GO

									--------------------------------------------------------------------
                                                        -- BẢNG TK KHÁCH HÀNG --
                                    --------------------------------------------------------------------
-- Dữ liệu Tài khoản Khách hàng
insert into tblTaiKhoanKH (MaTaiKhoanKH, TenTaiKhoan, MaKhachHang, MatKhau, Quyen, HinhAnh, TrangThaiTaiKhoan, NgayCapNhat)
values	('TKKH0001', 'an_nguyen', 'KH0001', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-11'),
		('TKKH0002', 'binh_tran', 'KH0002', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-11'),
		('TKKH0003', 'cuc_le', 'KH0003', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-11'),
		('TKKH0004', 'duy_pham', 'KH0004', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-11'),
		('TKKH0005', 'em_hoang', 'KH0005', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-11'),
		('TKKH0006', 'giang_vu', 'KH0007', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-12'),
		('TKKH0007', 'hao_bui', 'KH0008', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-12'),
		('TKKH0008', 'hien_ngo', 'KH0009', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-12'),
		('TKKH0009', 'khanh_dang', 'KH0010', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-12'),
		('TKKH0010', 'linh_nguyen', 'KH0011', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-12'),
		('TKKH0011', 'minh_tran', 'KH0012', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-12'),
		('TKKH0012', 'nga_le', 'KH0013', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-12'),
		('TKKH0013', 'oanh_pham', 'KH0014', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-12'),
		('TKKH0014', 'phat_hoang', 'KH0015', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-12'),
		('TKKH0015', 'son_vu', 'KH0017', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-15'),
		('TKKH0016', 'tuan_bui', 'KH0018', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-15'),
		('TKKH0017', 'uyen_ngo', 'KH0019', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-15'),
		('TKKH0018', 'van_dang', 'KH0020', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-15'),
		('TKKH0019', 'hoa_ly', 'KH0021', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-16'),
		('TKKH0020', 'thuy_phan', 'KH0023', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-16'),
		('TKKH0021', 'tam_ngo', 'KH0024', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-16'),
		('TKKH0022', 'phuc_doan', 'KH0025', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-16'),
		('TKKH0023', 'yen_lam', 'KH0026', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-16'),
		('TKKH0024', 'hieu_ta', 'KH0027', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-16'),
		('TKKH0025', 'bao_quach', 'KH0028', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-16'),
		('TKKH0026', 'dung_cao', 'KH0029', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-16'),
		('TKKH0027', 'lan_duong', 'KH0030', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-18'),
		('TKKH0028', 'huy_trinh', 'KH0031', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-18'),
		('TKKH0029', 'duong_ninh', 'KH0032', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-18'),
		('TKKH0030', 'tinh_kim', 'KH0033', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-22'),
		('TKKH0031', 'nhi_ngo', 'KH0034', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-22'),
		('TKKH0032', 'tuan_duong', 'KH0035', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-22'),
		('TKKH0033', 'anh_la', 'KH0036', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-22'),
		('TKKH0034', 'huy_mac', 'KH0037', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-23'),
		('TKKH0035', 'han_tieu', 'KH0038', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-23'),
		('TKKH0036', 'hoang_trieu', 'KH0039', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-23'),
		('TKKH0037', 'chau_cung', 'KH0040', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-27'),
		('TKKH0038', 'kien_thach', 'KH0041', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-27'),
		('TKKH0039', 'thao_tong', 'KH0042', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-27'),
		('TKKH0040', 'toan_vi', 'KH0043', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-27'),
		('TKKH0041', 'bich_lu', 'KH0044', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-27'),
		('TKKH0042', 'thu_lam', 'KH0045', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-27'),
		('TKKH0043', 'phuc_tu', 'KH0046', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-30'),
		('TKKH0044', 'hoa_huynh', 'KH0047', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-30'),
		('TKKH0045', 'khoi_ha', 'KH0048', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-30'),
		('TKKH0046', 'ha_luong', 'KH0049', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-30'),
		('TKKH0047', 'dung_trieu', 'KH0050', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-30'),
		('TKKH0048', 'mai_ly', 'KH0051', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-04-30'),
		('TKKH0049', 'linh_dang', 'KH0053', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-05-01'),
		('TKKH0050', 'son_bach', 'KH0054', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-05-01'),
		('TKKH0051', 'vy_trinh', 'KH0055', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-05-01'),
		('TKKH0052', 'nam_to', 'KH0056', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-05-01'),
		('TKKH0053', 'kim_ta', 'KH0057', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-05-01'),
		('TKKH0054', 'trang_nguyen', 'KH0059', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-05-02'),
		('TKKH0055', 'phat_le', 'KH0060', 'Kh123456@', N'Khách hàng', 'Avatar.png', 1, '2025-05-02');

-----------------------------------------------------------------
-- TRIGGER BẢNG TK KHÁCH HÀNG
-----------------------------------------------------------------
CREATE TRIGGER trg_NhapTaiKhoanKH_Validate
ON tblTaiKhoanKH
INSTEAD OF INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NewCode NCHAR(8);

    -----------------------------------------------------------------
    -- 1. Tự sinh mã tài khoản khách hàng (INSERT)
    -----------------------------------------------------------------
    IF EXISTS (SELECT 1 FROM inserted) AND NOT EXISTS (SELECT 1 FROM deleted)
    BEGIN
        DECLARE @LastCode NCHAR(8), @Num INT;

        SELECT @LastCode = MAX(MaTaiKhoanKH) FROM tblTaiKhoanKH;

        IF @LastCode IS NULL 
            SET @NewCode = 'TKKH0001';
        ELSE
        BEGIN
            SET @Num = CAST(SUBSTRING(@LastCode, 5, 4) AS INT) + 1;
            SET @NewCode = 'TKKH' + RIGHT('0000' + CAST(@Num AS VARCHAR(4)), 4);
        END
    END

    -----------------------------------------------------------------
    -- 2. Kiểm tra TÊN TÀI KHOẢN
    -----------------------------------------------------------------
    -- Chỉ chữ, số, _, ., -
    IF EXISTS (
        SELECT 1 FROM inserted
        WHERE TenTaiKhoan LIKE '%[^a-zA-Z0-9_.-]%'
    )
    BEGIN
        RAISERROR(N'Tên tài khoản chỉ được chứa chữ, số, dấu _ . -', 16, 1);
        RETURN;
    END

    -- Không trùng tên
    IF EXISTS (
        SELECT 1
        FROM inserted i
        WHERE EXISTS (
            SELECT 1 FROM tblTaiKhoanKH t
            WHERE t.TenTaiKhoan = i.TenTaiKhoan
              AND t.MaTaiKhoanKH <> i.MaTaiKhoanKH
        )
    )
    BEGIN
        RAISERROR(N'Tên tài khoản đã tồn tại.', 16, 1);
        RETURN;
    END

    -----------------------------------------------------------------
    -- 3. Kiểm tra MẬT KHẨU
    -----------------------------------------------------------------
    IF EXISTS (
        SELECT 1 FROM inserted
        WHERE LEN(MatKhau) < 8
           OR LEN(MatKhau) > 50
           OR MatKhau LIKE ' %' 
           OR MatKhau LIKE '% '
           OR MatKhau NOT LIKE '%[a-z]%'
           OR MatKhau NOT LIKE '%[A-Z]%'
           OR MatKhau NOT LIKE '%[0-9]%'
           OR MatKhau NOT LIKE '%[@\-\_\!\#\$\%\&\*]%'
           OR MatKhau LIKE '% %' 
           OR (MatKhau NOT LIKE '%[0-9]%' AND MatKhau LIKE '%[a-zA-Z]%')
           OR (MatKhau NOT LIKE '%[a-zA-Z]%' AND MatKhau LIKE '%[0-9]%')
           OR MatKhau LIKE '%aaaa%' OR MatKhau LIKE '%1111%' OR MatKhau LIKE '%123456%'
    )
    BEGIN
        RAISERROR(N'Mật khẩu không hợp lệ, vui lòng kiểm tra lại.', 16, 1);
        RETURN;
    END

    -----------------------------------------------------------------
    -- 4. INSERT / UPDATE
    -----------------------------------------------------------------
    IF NOT EXISTS (SELECT * FROM deleted)   -- INSERT
    BEGIN
        INSERT INTO tblTaiKhoanKH
            (MaTaiKhoanKH, TenTaiKhoan, MaKhachHang,
             MatKhau, Quyen, HinhAnh, TrangThaiTaiKhoan, NgayCapNhat)
        SELECT 
            @NewCode,
            TenTaiKhoan,
            MaKhachHang,
            MatKhau,
            N'Khách hàng',       -- ★ GÁN MẶC ĐỊNH
            HinhAnh,
            TrangThaiTaiKhoan,
            GETDATE()
        FROM inserted;
    END
    ELSE  -- UPDATE
    BEGIN
        UPDATE TK
        SET 
            TenTaiKhoan = i.TenTaiKhoan,
            MaKhachHang = i.MaKhachHang,
            MatKhau = i.MatKhau,
            Quyen = N'Khách hàng',     -- ★ LUÔN GÁN MẶC ĐỊNH
            HinhAnh = i.HinhAnh,
            TrangThaiTaiKhoan = i.TrangThaiTaiKhoan,
            NgayCapNhat = GETDATE()
        FROM tblTaiKhoanKH TK
        INNER JOIN inserted i ON TK.MaTaiKhoanKH = i.MaTaiKhoanKH;
    END
END
GO


									--------------------------------------------------------------------
                                                        -- BẢNG NHÀ CUNG CẤP --
                                    --------------------------------------------------------------------
-- Dữ liệu nhà cung cấp
insert into tblNhaCungCap (MaNhaCungCap, TenNhaCungCap, DiaChi, DienThoai, Email)
values	('NCC001', N'Công ty TNHH L’Oréal Việt Nam', N'The Nexus, 3b Tôn Đức Thắng, Phường Sài Gòn, TP.HCM', '1800545463', 'lorealvietnam@loreal.com'),
		('NCC002', N'Công ty TNHH Mỹ Phẩm Shiseido Việt Nam', N'Lầu 27, Vietcombank Tower, số 5, Công trường Mê Linh, Phường Sài Gòn, TP.HCM', '02839101222', 'chamsockhachhang@scv.shiseido.vn'),
		('NCC003', N'Công ty TNHH Quốc Tế Unilever Việt Nam', N'Lô A2-3 KCN Tây Bắc Củ Chi, xã Tân An Hội, Củ Chi, TP.HCM', '02854135686', 'tuvankhachhang@unilever.com'),
		('NCC004', N'Công Ty TNHH Procter & Gamble Việt Nam', N'Khu công nghiệp Đồng An, Phường Bình Hòa, TP.HCM', '02835214555', 'pgvn@pg.com'),
		('NCC005', N'Công Ty TNHH Beiersdorf Việt Nam', N'Lầu 18, 72-74 Nguyễn Thị Minh Khai, Phường Xuân Hòa, TP.HCM', '0283939 3921', 'Phuong.DoThiNgoc@beiersdorf.com'),
		('NCC006', N'Công Ty TNHH Amorepacific Việt Nam', N'Lầu 4A, Vincom Center, 72 Lê Thánh Tôn, phường Sài Gòn, TP.HCM', '02838246232', 'loanvth@vn.amorepacific.com'),
		('NCC007', N'Công ty TNHH Mỹ Phẩm LG VINA', N'Tòa nhà Empress Tower, số 138-142, đường Hai Bà Trưng, Phường Tân Định, TP.HCM', '02835219040', 'thupth@lgcare.com.vn'),
		('NCC008', N'Công ty Cổ phần Thương mại Dịch vụ Tổng hợp Hoàn Vũ', N'Tầng 7, Tòa nhà Friendship, Số 31, Đường Lê Duẩn, Phường Sài Gòn, TP.HCM', '18006035', 'customercare@hsvgroup.com.vn'),
		('NCC009', N'Công ty TNHH Nature Story', N'38C-39C Khu phố 1, Quốc Lộ 1A, Phường Tân Thới Hiệp, TP.HCM', '19009300', 'we@cocoonvietnam.com'),
		('NCC010', N'Công TNHH Galderma Việt Nam', N'Tầng 16, số 33 Lê Duẩn, Phường Sài Gòn, TP.HCM', ' 0908170810', ' pharmacovigilance.vn@galderma.com');

---------------------------------------------------------------------
-- TRIGGER NHÀ CUNG CẤP
---------------------------------------------------------------------

CREATE OR ALTER TRIGGER trg_NhapNhaCungCap_InsertUpdate
ON tblNhaCungCap
INSTEAD OF INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    ---------------------------------------------------------------------
    -- 1. Lấy dữ liệu mới
    ---------------------------------------------------------------------
    DECLARE 
        @Ten NVARCHAR(60),
        @DiaChi NVARCHAR(150),
        @DienThoai NVARCHAR(15),
        @Email NVARCHAR(40),
        @FormattedTen NVARCHAR(60),
        @LocalPart NVARCHAR(50),
        @Domain NVARCHAR(50),
        @DTclean VARCHAR(20);

    SELECT 
        @Ten = TenNhaCungCap,
        @DiaChi = DiaChi,
        @DienThoai = DienThoai,
        @Email = Email
    FROM inserted;

    ---------------------------------------------------------------------
    -- 2. Validate TÊN NHÀ CUNG CẤP
    ---------------------------------------------------------------------
    IF @Ten LIKE '%[^A-Za-z0-9 ]%'
    BEGIN
        RAISERROR (N'Tên nhà cung cấp chỉ được chứa chữ, số và khoảng trắng.', 16, 1);
        RETURN;
    END;

    IF @Ten LIKE '[0-9]%'
    BEGIN
        RAISERROR (N'Tên nhà cung cấp không được bắt đầu bằng chữ số.', 16, 1);
        RETURN;
    END;

    -- Viết hoa chữ cái đầu tiên toàn chuỗi
    SET @FormattedTen = UPPER(LEFT(@Ten, 1)) + LOWER(SUBSTRING(@Ten, 2, LEN(@Ten)));

    ---------------------------------------------------------------------
    -- 3. Validate ĐỊA CHỈ
    ---------------------------------------------------------------------
    IF @DiaChi LIKE '%[^A-Za-z0-9/., ]%'
    BEGIN
        RAISERROR (N'Địa chỉ chỉ được chứa chữ, số, dấu "/ . ," và khoảng trắng.', 16, 1);
        RETURN;
    END;

    IF LEFT(@DiaChi,1) NOT LIKE '[A-Za-z0-9]'
    BEGIN
        RAISERROR (N'Địa chỉ phải bắt đầu bằng chữ hoặc số.', 16, 1);
        RETURN;
    END;

    -- Kiểm tra dấu "/" phải nằm giữa hai số
     IF @DiaChi LIKE '%[^0-9]/%' OR @DiaChi LIKE '%/[^0-9]%' 
       OR @DiaChi LIKE '/%' OR @DiaChi LIKE '%/'
    BEGIN
        RAISERROR (N'Địa chỉ không hợp lệ: dấu "/" phải nằm giữa hai chữ số.', 16, 1);
        RETURN;
    END;

    ---------------------------------------------------------------------
    -- 4. Validate ĐIỆN THOẠI
    ---------------------------------------------------------------------

    -- Không chứa ký tự không phải số
    IF @DienThoai LIKE '%[^0-9()% -]%'
    BEGIN
        RAISERROR (N'Điện thoại chỉ được chứa số (và () cho số cố định).', 16, 1);
        RETURN;
    END;

    -- Không được có khoảng trắng
    IF @DienThoai LIKE '% %'
    BEGIN
        RAISERROR (N'Điện thoại không được chứa khoảng trắng.', 16, 1);
        RETURN;
    END;

    -- Xóa ký tự để kiểm tra độ dài thực
    SET @DTclean = @DienThoai;
    SET @DTclean = REPLACE(@DTclean, '(', '');
    SET @DTclean = REPLACE(@DTclean, ')', '');
    SET @DTclean = REPLACE(@DTclean, '-', '');

    IF LEN(@DTclean) < 7 OR LEN(@DTclean) > 12
    BEGIN
        RAISERROR (N'Điện thoại quá ngắn hoặc quá dài.', 16, 1);
        RETURN;
    END;

    ---- HOTLINE 1800 / 1900 (7–8 số, bắt buộc)
    IF @DTclean LIKE '18%' OR @DTclean LIKE '19%'
    BEGIN
        IF @DTclean NOT LIKE '1800%' AND @DTclean NOT LIKE '1900%'
        BEGIN
            RAISERROR (N'Hotline phải bắt đầu bằng 1800 hoặc 1900.', 16, 1);
            RETURN;
        END;

        IF LEN(@DTclean) NOT BETWEEN 7 AND 8
        BEGIN
            RAISERROR (N'Hotline phải có từ 7–8 chữ số.', 16, 1);
            RETURN;
        END;
    END;

    ---------------------------------------------------------------------
    -- Kiểm tra SỐ DI ĐỘNG (10 số, đầu số hợp lệ)
    ---------------------------------------------------------------------
    IF LEN(@DTclean) = 10 AND 
       @DTclean NOT LIKE '03%' AND
       @DTclean NOT LIKE '05%' AND
       @DTclean NOT LIKE '07%' AND
       @DTclean NOT LIKE '08%' AND
       @DTclean NOT LIKE '09%'
    BEGIN
        RAISERROR (N'Số di động không hợp lệ (đầu số sai).', 16, 1);
        RETURN;
    END;
       
    ---------------------------------------------------------------------
    -- 5. Validate EMAIL
    ---------------------------------------------------------------------
    -- Không chứa khoảng trắng
    IF @Email LIKE '% %'
    BEGIN
        RAISERROR (N'Email không được chứa khoảng trắng.', 16, 1);
        RETURN;
    END;

    -- Phải có @ và dấu chấm sau @
    IF @Email NOT LIKE '%@%.%'
    BEGIN
        RAISERROR (N'Email không hợp lệ (thiếu @ hoặc .).', 16, 1);
        RETURN;
    END;

    -- Không cho phép dấu chấm ngay trước @
    IF @Email LIKE '%.@%'
    BEGIN
        RAISERROR (N'Email không hợp lệ (có dấu chấm ngay trước @).', 16, 1);
        RETURN;
    END;

    -- Chỉ duy nhất 1 ký tự @
    IF LEN(@Email) - LEN(REPLACE(@Email, '@', '')) <> 1
    BEGIN
        RAISERROR (N'Email phải chứa duy nhất 1 ký tự @.', 16, 1);
        RETURN;
    END;

    -- Tách local & domain
    SET @LocalPart = LEFT(@Email, CHARINDEX('@', @Email)-1);
    SET @Domain = SUBSTRING(@Email, CHARINDEX('@', @Email)+1, LEN(@Email));

    -- Local part không được rỗng
    IF LEN(@LocalPart) = 0
    BEGIN
        RAISERROR (N'Email không hợp lệ (phần tên bị rỗng).', 16, 1);
        RETURN;
    END;

    -- Không bắt đầu bằng dấu chấm
    IF @LocalPart LIKE '.%'
    BEGIN
        RAISERROR (N'Email không được bắt đầu bằng dấu chấm.', 16, 1);
        RETURN;
    END;

    -- Không cho hai dấu chấm liên tiếp
    IF @Email LIKE '%..%'
    BEGIN
        RAISERROR (N'Email không hợp lệ (chứa ..).', 16, 1);
        RETURN;
    END;

    -- Domain phải chứa dấu chấm
    IF @Domain NOT LIKE '%._%' AND @Domain NOT LIKE '%.%_%'
       AND CHARINDEX('.', @Domain) = 0
    BEGIN
        RAISERROR (N'Domain email phải chứa dấu chấm.', 16, 1);
        RETURN;
    END;

    -- Domain chỉ được chứa chữ, số, dấu trừ và dấu chấm
    IF @Domain LIKE '%[^A-Za-z0-9.-]%'
    BEGIN
        RAISERROR (N'Domain email không hợp lệ (chỉ cho phép chữ, số, ., -).', 16, 1);
        RETURN;
    END;

    -- Domain không bắt đầu hoặc kết thúc bằng "-"
    IF @Domain LIKE '-%' OR @Domain LIKE '%-'
    BEGIN
        RAISERROR (N'Domain email không được bắt đầu hoặc kết thúc bằng "-".', 16, 1);
        RETURN;
    END;

    ---------------------------------------------------------------------
    -- 6. INSERT – tự sinh mã NCCxxx
    ---------------------------------------------------------------------
    IF NOT EXISTS (
        SELECT NCC.MaNhaCungCap 
        FROM tblNhaCungCap NCC
        JOIN inserted i ON NCC.MaNhaCungCap = i.MaNhaCungCap
    )
    BEGIN
        DECLARE @NewID nchar(6), @MaxID int;

        SELECT @MaxID = MAX(CAST(SUBSTRING(MaNhaCungCap, 4, 3) AS int))
        FROM tblNhaCungCap;

        SET @MaxID = ISNULL(@MaxID, 0) + 1;

        SET @NewID = 'NCC' + RIGHT('000' + CAST(@MaxID AS varchar(3)), 3);

        INSERT INTO tblNhaCungCap (MaNhaCungCap, TenNhaCungCap, DiaChi, DienThoai, Email)
        SELECT @NewID, @FormattedTen, @DiaChi, @DienThoai, @Email;

        RETURN;
    END;

    ---------------------------------------------------------------------
    -- 7. UPDATE – chỉ update tên, địa chỉ, điện thoại, email
    ---------------------------------------------------------------------
    UPDATE NCC
    SET 
        TenNhaCungCap = @FormattedTen,
        DiaChi = @DiaChi,
        DienThoai = @DienThoai,
        Email = @Email
    FROM tblNhaCungCap NCC
    JOIN inserted i ON NCC.MaNhaCungCap = i.MaNhaCungCap;

END;
GO

									--------------------------------------------------------------------
                                                        -- BẢNG LOẠI SẢN PHẨM --
                                    --------------------------------------------------------------------
--Dữ liệu Loại sản phẩm 
insert into tblLoaiSanPham (MaLoaiSanPham, TenLoaiSanPham)
values	('LSP001', N'Trang điểm'),
		('LSP002', N'Chăm sóc da'),
		('LSP003', N'Chăm sóc sức khỏe'),
		('LSP004', N'Chăm sóc cá nhân'),
		('LSP005', N'Chăm sóc tóc'),
		('LSP006', N'Trẻ em'), 
		('LSP007', N'Nam giới'),
		('LSP008', N'Chăm sóc nhà cửa');

--Trigger
CREATE TRIGGER trg_NhapLoaiSanPham_InsertUpdate
ON tblLoaiSanPham
INSTEAD OF INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -------------------------------------------------------------
    -- 1. Kiểm tra ký tự hợp lệ (A-Z, a-z, 0-9, khoảng trắng)
    -------------------------------------------------------------
    IF EXISTS (
        SELECT 1
        FROM inserted
        WHERE TenLoaiSanPham LIKE '%[^A-Za-z0-9 ]%'
    )
    BEGIN
        RAISERROR (N'Tên loại sản phẩm chỉ được chứa chữ, số và khoảng trắng.', 16, 1);
        RETURN;
    END;

    -------------------------------------------------------------
    -- 2. Không được bắt đầu bằng số
    -------------------------------------------------------------
    IF EXISTS (
        SELECT 1
        FROM inserted
        WHERE TenLoaiSanPham LIKE '[0-9]%'
    )
    BEGIN
        RAISERROR (N'Tên loại sản phẩm không được bắt đầu bằng số.', 16, 1);
        RETURN;
    END;

    -------------------------------------------------------------
    -- 3. Chuẩn hóa: viết hoa chữ cái đầu tiên, còn lại viết thường
    -------------------------------------------------------------
    DECLARE @Ten NVARCHAR(50);
    DECLARE @Formatted NVARCHAR(50);

    SELECT @Ten = TenLoaiSanPham FROM inserted;

    SET @Formatted = UPPER(LEFT(@Ten, 1)) + LOWER(SUBSTRING(@Ten, 2, LEN(@Ten)));

    -------------------------------------------------------------
    -- 4. Xử lý INSERT: tự sinh mã LSPxxx
    -------------------------------------------------------------
    IF NOT EXISTS (
        SELECT TH.MaLoaiSanPham
        FROM tblLoaiSanPham TH
        JOIN inserted i ON TH.MaLoaiSanPham = i.MaLoaiSanPham
    )
    BEGIN
        DECLARE @NewID nchar(6);
        DECLARE @MaxID int;

        SELECT @MaxID = MAX(CAST(SUBSTRING(MaLoaiSanPham, 4, 3) AS int))
        FROM tblLoaiSanPham;

        SET @MaxID = ISNULL(@MaxID, 0) + 1;

        SET @NewID = 'LSP' + RIGHT('000' + CAST(@MaxID AS varchar(3)), 3);

        INSERT INTO tblLoaiSanPham (MaLoaiSanPham, TenLoaiSanPham)
        SELECT @NewID, @Formatted;

        RETURN;
    END;

    -------------------------------------------------------------
    -- 5. Xử lý UPDATE: chỉ cập nhật tên, mã không thay đổi
    -------------------------------------------------------------
    UPDATE LSP
    SET LSP.TenLoaiSanPham = @Formatted
    FROM tblLoaiSanPham LSP
    JOIN inserted i ON LSP.MaLoaiSanPham = i.MaLoaiSanPham;

END;
GO

									--------------------------------------------------------------------
                                                        -- BẢNG THƯƠNG HIỆU --
                                    --------------------------------------------------------------------
-- Dữ liệu Thương hiệu
insert into tblThuongHieu (MaThuongHieu, TenThuongHieu)
values	('TH0001', N'L’Oréal Paris'), 
		('TH0002', N'Maybelline'), 
		('TH0003', N'La Roche-Posay'),
		('TH0004', N'CeraVe'),
		('TH0005', N'3CE'), 
		('TH0006', N'Anessa'), 
		('TH0007', N'Dove'), 
		('TH0008', N'Closeup'),
		('TH0009', N'TRESemmé'),
		('TH0010', N'Vaseline'),
		('TH0011', N'Lifebuoy'),
		('TH0012', N'St.Ives'),
		('TH0013', N'Head & Shoulders'), 
		('TH0014', N'Gillette'),
		('TH0015', N'Downy'),
		('TH0016', N'NIVEA'), 
		('TH0017', N'NIVEA Men'),
		('TH0018', N'Laneige'), 
		('TH0019', N'Innisfree'), 
		('TH0020', N'O HUI'), 
		('TH0021', N'Clio Professional'), 
		('TH0022', N'Peripera'),
		('TH0023', N'DHC'), 
		('TH0024', N'Cocon'), 
		('TH0025', N'Cetaphil');

---------------------------------------------------------------------
-- TRIGGER BẢNG THƯƠNG HIỆU
---------------------------------------------------------------------
CREATE TRIGGER trg_NhapThuongHieu_InsertUpdate
ON tblThuongHieu
INSTEAD OF INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -------------------------------------------------------------
    -- 1. Kiểm tra ký tự hợp lệ cho TenThuongHieu
    -------------------------------------------------------------
    IF EXISTS (
        SELECT 1
        FROM inserted
        WHERE TenThuongHieu LIKE '%[^A-Za-z0-9 _-]%'
    )
    BEGIN
        RAISERROR (N'Tên thương hiệu chỉ được phép chứa chữ, số, khoảng trắng, "-", "_"', 16, 1);
        RETURN;
    END;


    -------------------------------------------------------------
    -- 2. Xử lý thêm mới (INSERT)
    -------------------------------------------------------------
    IF NOT EXISTS (
        SELECT TH.MaThuongHieu
        FROM tblThuongHieu TH
        JOIN inserted i ON TH.MaThuongHieu = i.MaThuongHieu
    )
    BEGIN
        DECLARE @NewID nchar(6);
        DECLARE @MaxID int;

        SELECT @MaxID = MAX(CAST(SUBSTRING(MaThuongHieu, 3, 4) AS int))
        FROM tblThuongHieu;

        SET @MaxID = ISNULL(@MaxID, 0) + 1;

        SET @NewID = 'TH' + RIGHT('0000' + CAST(@MaxID AS varchar(4)), 4);

        INSERT INTO tblThuongHieu (MaThuongHieu, TenThuongHieu)
        SELECT @NewID, TenThuongHieu
        FROM inserted;

        RETURN;
    END;


    -------------------------------------------------------------
    -- 3. Xử lý cập nhật TenThuongHieu (UPDATE)
    -------------------------------------------------------------
    UPDATE TH
    SET TH.TenThuongHieu = i.TenThuongHieu
    FROM tblThuongHieu TH
    JOIN inserted i ON TH.MaThuongHieu = i.MaThuongHieu;

END;
GO

									--------------------------------------------------------------------
															-- BẢNG SẢN PHẨM --
                                    --------------------------------------------------------------------
-- Hàm tính checksum (Giữ nguyên)
CREATE FUNCTION dbo.EAN13_MaVach(@code12 varchar(12))
RETURNS char(1)
AS
BEGIN
    DECLARE @sum int = 0, @i int = 1, @digit int, @posFromRight int;
    WHILE @i <= 12
    BEGIN
        SET @digit = ASCII(SUBSTRING(@code12, @i, 1)) - 48;
        SET @posFromRight = 12 - @i + 1;
        IF (@posFromRight % 2 = 0)
            SET @sum += @digit * 3;
        ELSE
            SET @sum += @digit;
        SET @i += 1;
    END
    DECLARE @check int = (10 - (@sum % 10)) % 10;
    RETURN CHAR(@check + 48);
END;
GO

-- Tạo lại Trigger đã sửa đổi
CREATE TRIGGER trg_Auto_MaVach
ON tblSanPham
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

	UPDATE sp
	SET MaVach = x.code12 + dbo.EAN13_MaVach(x.code12)
	FROM tblSanPham sp
	JOIN (
		SELECT 
			i.MaSanPham,
			-- SỬ DỤNG ISNULL VÀ RIGHT() ĐỂ ĐẢM BẢO CHUỖI 12 KÝ TỰ KHÔNG BAO GIỜ BỊ NULL
			code12 =
				'893' +
				RIGHT(ISNULL(i.MaThuongHieu, 'TH0000'), 3) + -- Nếu NULL/'' là TH0000
				RIGHT(ISNULL(i.MaLoaiSanPham, 'LSP000'), 2) + -- Nếu NULL/'' là LSP000
				RIGHT(i.MaSanPham, 4)
		FROM inserted i
        WHERE i.MaVach = '' OR i.MaVach IS NULL
	) x ON sp.MaSanPham = x.MaSanPham
    WHERE sp.MaVach = '' OR sp.MaVach IS NULL;
END;
GO

---------------------------------------------------------------------
-- TRIGGER TỰ SINH MASANPHAM + VALIDATE TENSANPHAM + MOTA
---------------------------------------------------------------------
CREATE TRIGGER trg_NhapSanPham_InsertUpdate
ON tblSanPham
INSTEAD OF INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @MaSanPham nchar(6),
        @TenSanPham nvarchar(150),
        @MaLoaiSanPham nchar(6),
        @MaThuongHieu nchar(6),
        @DonViTinh nvarchar(30),
        @GiaBan decimal(18,2),
        @MoTa nvarchar(255),
        @TrangThaiSanPham bit,
        @HinhAnh nvarchar(255),
        @MaVach varchar(13),
        @MaNhaCungCap nchar(6),
        @MaNhanVien nchar(6),
        @NgayCapNhat datetime;

    -- Lấy dữ liệu từ inserted
    SELECT 
        @MaSanPham = MaSanPham,
        @TenSanPham = TenSanPham,
        @MaLoaiSanPham = MaLoaiSanPham,
        @MaThuongHieu = MaThuongHieu,
        @DonViTinh = DonViTinh,
        @GiaBan = GiaBan,
        @MoTa = MoTa,
        @TrangThaiSanPham = TrangThaiSanPham,
        @HinhAnh = HinhAnh,
        @MaVach = MaVach,
        @MaNhaCungCap = MaNhaCungCap,
        @MaNhanVien = MaNhanVien,
        @NgayCapNhat = NgayCapNhat
    FROM inserted;

    ---------------------------------------------------------------------
    -- 1) Xử lý tự sinh mã (chỉ khi INSERT)
    ---------------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM tblSanPham WHERE MaSanPham = @MaSanPham)
    BEGIN
        DECLARE @SoCuoi int = (
            SELECT MAX(CAST(SUBSTRING(MaSanPham, 3, 4) AS int))
            FROM tblSanPham
        );
        IF @SoCuoi IS NULL SET @SoCuoi = 0;

        SET @MaSanPham = 'SP' + RIGHT('000' + CAST(@SoCuoi + 1 AS varchar(4)), 4);
    END
    ELSE
    BEGIN
        -- Trường hợp UPDATE: Không cho sửa mã
        IF EXISTS (
            SELECT 1 
            FROM inserted i
            JOIN deleted d ON i.MaSanPham <> d.MaSanPham
        )
        BEGIN
            RAISERROR('MaSanPham không được phép chỉnh sửa.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END
    END

    ---------------------------------------------------------------------
    -- 2) VALIDATE TenSanPham
    ---------------------------------------------------------------------
    IF (@TenSanPham LIKE '[0-9]%' )
    BEGIN
        RAISERROR('TenSanPham không được bắt đầu bằng chữ số.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- Chỉ chữ cái, số, khoảng trắng
    IF (@TenSanPham LIKE '%[^a-zA-Z0-9 ]%')
    BEGIN
        RAISERROR('TenSanPham chỉ được chứa chữ cái, số và khoảng trắng.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- Viết hoa chữ cái đầu nhưng không viết hoa toàn bộ từng từ
    SET @TenSanPham = UPPER(LEFT(@TenSanPham, 1)) + LOWER(SUBSTRING(@TenSanPham, 2, LEN(@TenSanPham)));

    ---------------------------------------------------------------------
    -- 3) VALIDATE MoTa (cho phép ký tự mở rộng)
    ---------------------------------------------------------------------
    IF @MoTa IS NOT NULL
    BEGIN
        -- Chỉ cho phép: chữ, số, khoảng trắng, các ký tự: . , ! ? : ; % + - * / ₫ $ € () [] {} _ xuống dòng
        IF @MoTa LIKE '%[^a-zA-Z0-9 .,!?;:%+\-*/₫$€()\[\]\{\}_\n\r]%'
        BEGIN
            RAISERROR('MoTa chứa ký tự không hợp lệ.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Viết hoa chữ cái đầu
        SET @MoTa = UPPER(LEFT(@MoTa, 1)) + LOWER(SUBSTRING(@MoTa, 2, LEN(@MoTa)));
    END

    ---------------------------------------------------------------------
    -- 4) Insert hoặc Update vào bảng thật
    ---------------------------------------------------------------------
    IF EXISTS (SELECT 1 FROM tblSanPham WHERE MaSanPham = @MaSanPham)
    BEGIN
        UPDATE tblSanPham
        SET
            TenSanPham = @TenSanPham,
            MaLoaiSanPham = @MaLoaiSanPham,
            MaThuongHieu = @MaThuongHieu,
            DonViTinh = @DonViTinh,
            GiaBan = @GiaBan,
            MoTa = @MoTa,
            TrangThaiSanPham = @TrangThaiSanPham,
            HinhAnh = @HinhAnh,
            MaVach = @MaVach,
            MaNhaCungCap = @MaNhaCungCap,
            MaNhanVien = @MaNhanVien,
            NgayCapNhat = @NgayCapNhat
        WHERE MaSanPham = @MaSanPham;
    END
    ELSE
    BEGIN
        INSERT INTO tblSanPham(
            MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan,
            MoTa, TrangThaiSanPham, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat
        )
        VALUES (
            @MaSanPham, @TenSanPham, @MaLoaiSanPham, @MaThuongHieu, @DonViTinh, @GiaBan,
            @MoTa, @TrangThaiSanPham, @HinhAnh, @MaVach, @MaNhaCungCap, @MaNhanVien, @NgayCapNhat
        );
    END
END;
GO

CREATE TRIGGER trg_SanPham_TuCapNhatNgay
ON tblSanPham
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Tự động cập nhật NgayCapNhat = thời điểm hiện tại
    UPDATE SP
    SET NgayCapNhat = GETDATE()
    FROM tblSanPham SP
    INNER JOIN inserted i ON SP.MaSanPham = i.MaSanPham;
END;
GO

ALTER TABLE tblSanPham
ALTER COLUMN MaVach varchar(13) NULL;

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0001', N'Phấn Má Hồng 3CE Blushlighter 9g', 'LSP001', 'TH0005', N'Hộp', 448000, null, 'SP01.png', ' ', 'NCC001', 'NV0001', '2025-02-14');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0002', N'Má Hồng Kem Clio Essential Lipcheek Tap 4.5g', 'LSP001', 'TH0021', N'Hộp', 249000, null, 'SP02.png', ' ', 'NCC008', 'NV0001', '2025-02-14');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0003', N'Son Peripera Ink The Jellable Tint 3.7g', 'LSP001', 'TH0022', N'Thỏi', 280000, null, 'SP03.png', ' ', 'NCC008', 'NV0001', '2025-02-28'); -- ĐÃ SỬA NGÀY

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0004', N'Kem Nền L’Oréal Infallible 24H Tinted Serum 30ml', 'LSP001', 'TH0001', N'Chai', 429000, null, 'SP04.png', ' ', 'NCC001', 'NV0001', '2025-02-14');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0005', N'Mascara Maybelline New York Hypercurl 9.2ml', 'LSP001', 'TH0002', N'Cây', 168000, null, 'SP05.png', ' ', 'NCC001', 'NV0001', '2025-02-14');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0006', N'Cushion LANEIGE Căng Bóng Neo Mewy 15g', 'LSP001', 'TH0018', N'Hộp', 885000, null, 'SP06.png', ' ', 'NCC006', 'NV0001', '2025-02-14');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0007', N'Phấn Phủ Bột innisfree No Sebum 5g', 'LSP001', 'TH0019', N'Hộp', 180000, null, 'SP07.png', ' ', 'NCC006', 'NV0001', '2025-02-14');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0008', N'Bảng Phấn Trang Điểm 3CE Layer It All', 'LSP001', 'TH0005', N'Hộp', 550000, null, 'SP08.png', ' ', 'NCC001', 'NV0001', '2025-02-14');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0009', N'Son Kem Bóng Maybelline Vinyl Ink 4.2ml', 'LSP001', 'TH0002', N'Thỏi', 298000, null, 'SP09.png', ' ', 'NCC001', 'NV0001', '2025-02-14');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0010', N'Sữa Rửa Mặt CeraVe Sạch Sâu Da Dầu 236ml', 'LSP002', 'TH0004', N'Tuýp', 350000, null, 'SP010.png', ' ', 'NCC001', 'NV0010', '2025-02-14');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0011', N'Sữa Chống Nắng Anessa Dưỡng Da Kiềm Dầu 60ml', 'LSP002', 'TH0006', N'Tuýp', 702000, null, 'SP011.png', ' ', 'NCC002', 'NV0010', '2025-02-15');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0012', N'Kem Dưỡng La Roche Posay Phục Hồi B5+ 40ml', 'LSP002', 'TH0003', N'Tuýp', 440000, null, 'SP012.png', ' ', 'NCC001', 'NV0010', '2025-02-15');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0013', N'Nước Tẩy Trang Sen Hậu Giang 500ml', 'LSP002', 'TH0024', N'Chai', 309000, null, 'SP013.png', ' ', 'NCC009', 'NV0010', '2025-02-15');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0014', N'Mặt Nạ Ngủ Laneige Bouncy & Firm Sleeping Mask 60ml', 'LSP002', 'TH0018', N'Hũ', 415000, null, 'SP014.png', ' ', 'NCC006', 'NV0010', '2025-02-15');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0015', N'Nước Cân Bằng Dưỡng Trắng Da OHUI Softener 150ml', 'LSP002', 'TH0020', N'Chai', 1350000, null, 'SP015.png', ' ', 'NCC007', 'NV0010', '2025-02-15');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0016', N'Kem Chống Nắng Nâng Tông La Roche-Posay SPF 50+ 50ml', 'LSP002', 'TH0003', N'Tuýp', 499000, null, 'SP016.png', ' ', 'NCC001', 'NV0010', '2025-02-15');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0017', N'Tinh Chất innisfree Green Tea Seed 30ml', 'LSP002', 'TH0019', N'Chai', 295000, null, 'SP017.png', ' ', 'NCC006', 'NV0010', '2025-02-15');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0018', N'Mặt Nạ Nghệ Hưng Yên Cocoon 30ml', 'LSP002', 'TH0024', N'Hũ', 143000, null, 'SP018.png', ' ', 'NCC009', 'NV0010', '2025-02-15');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0019', N'Mặt Nạ Innisfree Kombucha Energy Mask 22ml', 'LSP002', 'TH0019', N'Miếng', 36000, null, 'SP019.png', ' ', 'NCC006', 'NV0010', '2025-02-15');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0020', N'Dưỡng Môi Vaseline Lip Therapy Creme Brulle 7g', 'LSP002', 'TH0010', N'Hũ', 89000, null, 'SP020.png', ' ', 'NCC003', 'NV0010', '2025-02-15');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0021', N'Sữa Chống Nắng L’Oréal Paris Uv 50ml', 'LSP002', 'TH0001', N'Tuýp', 299000, null, 'SP021.png', ' ', 'NCC001', 'NV0019', '2025-02-17');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0022', N'Gel Cerave Giảm Mụn Blemish Control Gel 40ml', 'LSP002', 'TH0004', N'Tuýp', 329000, null, 'SP022.png', ' ', 'NCC001', 'NV0019', '2025-02-17');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0023', N'Thực phẩm bảo vệ sức khỏe DHC Vitamin C', 'LSP003', 'TH0023', N'Gói', 125000, null, 'SP023.png', ' ', 'NCC008', 'NV0019', '2025-02-17');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0024', N'Thực phẩm bảo vệ sức khỏe DHC Zinc', 'LSP003', 'TH0023', N'Gói', 140000, null, 'SP024.png', ' ', 'NCC008', 'NV0019', '2025-02-17');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0025', N'Thực phẩm bảo vệ sức khỏe DHC Collagen 7000 Plus', 'LSP003', 'TH0023', N'Hộp', 852000, null, 'SP025.png', ' ', 'NCC008', 'NV0019', '2025-02-17');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0026', N'Tẩy Da Chết Cocoon Dak Lak 200ml', 'LSP004', 'TH0024', N'Hũ', 123000, null, 'SP07.png', ' ', 'NCC009', 'NV0019', '2025-02-17');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0027', N'Sữa Tắm Dưỡng Ẩm Dove Hydration Boost 547ml', 'LSP004', 'TH0007', N'Chai', 246000, null, 'SP027.png', ' ', 'NCC003', 'NV0019', '2025-02-17');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0028', N'Sữa Tắm Serum Dove Bơ Hạt Mỡ & Vanilla 500g', 'LSP004', 'TH0007', N'Chai', 151000, null, 'SP028.png', ' ', 'NCC003', 'NV0019', '2025-02-17');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0029', N'Sữa Tắm ST.Ives Muối Biển 450ml', 'LSP004', 'TH0012', N'Chai', 150000, null, 'SP029.png', ' ', 'NCC003', 'NV0019', '2025-02-17');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0030', N'Sữa Dưỡng Thể Vaseline Gluta-Hya Nâng Tông 300ml', 'LSP004', 'TH0010', N'Tuýp', 187000, null, 'SP030.png', ' ', 'NCC003', 'NV0019', '2025-02-17'); -- ĐÃ SỬA: Mã TH0010

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0031', N'Sữa Dưỡng Thể Nivea Intensive Moisture 550ml', 'LSP004', 'TH0016', N'Chai', 189000, null, 'SP031.png', ' ', 'NCC005', 'NV0027', '2025-02-18');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0032', N'Nước Rửa Tay Lifebuoy Bảo Vệ Vượt Trội 450g', 'LSP004', 'TH0011', N'Chai', 78000, null, 'SP032.png', ' ', 'NCC003', 'NV0027', '2025-02-18');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0033', N'Kem Đánh Răng Closeup White Diamond Trắng Sáng 100g', 'LSP004', 'TH0008', N'Tuýp', 39000, null, 'SP033.png', ' ', 'NCC003', 'NV0027', '2025-02-18');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0034', N'Dầu Gội Head & Shoulders Smooth & Silky 2in1 370ml', 'LSP005', 'TH0013', N'Chai', 209000, null, 'SP034.png', ' ', 'NCC004', 'NV0027', '2025-02-18');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0035', N'Dầu Gội Dove Matcha Và Hoa Anh Đào 621ml', 'LSP005', 'TH0007', N'Chai', 206000, null, 'SP035.png', ' ', 'NCC003', 'NV0027', '2025-02-18');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0036', N'Dầu Gội L’Oréal Elseve Hyaluron Pure 72H 620ml', 'LSP005', 'TH0001', N'Chai', 199000, null, 'SP036.png', ' ', 'NCC001', 'NV0027', '2025-02-18');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0037', N'Kem Xả Serum Dove Ceramide Phục Hồi Hư Tổn 300g', 'LSP005', 'TH0007', N'Tuýp', 106000, null, 'SP037.png', ' ', 'NCC003', 'NV0027', '2025-02-18');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0038', N'Kem Ủ Tresemmé Tinh Dầu Argan & Keratin 180ml', 'LSP005', 'TH0009', N'Hũ', 175000, null, 'SP038.png', ' ', 'NCC003', 'NV0027', '2025-02-18');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0039', N'Dầu Xả L’Oréal Paris Extraordinary Oil 440ml', 'LSP005', 'TH0001', N'Chai', 259000, null, 'SP039.png', ' ', 'NCC001', 'NV0027', '2025-02-18');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0040', N'Dầu Dưỡng Tóc L’Oréal Paris Hoa Hồng 100ml', 'LSP005', 'TH0001', N'Chai', 289000, null, 'SP040.png', ' ', 'NCC001', 'NV0027', '2025-02-18');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0041', N'Kem Nhuộm L’Oréal Paris Ash Supreme 172ml', 'LSP005', 'TH0001', N'Hộp', 229000, null, 'SP041.png', ' ', 'NCC001', 'NV0035', '2025-02-20');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0042', N'Kem Dưỡng Trẻ Em Cetaphil Baby Daily Lotion 400ml', 'LSP006', 'TH0025', N'Chai', 270000, null, 'SP042.png', ' ', 'NCC010', 'NV0035', '2025-02-20');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0043', N'Sữa Chống Nắng Cetaphil Sun Kids SPF 50+ 150ml', 'LSP006', 'TH0025', N'Chai', 659000, null, 'SP043.png', ' ', 'NCC010', 'NV0035', '2025-02-20');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0044', N'Dầu Gội Trẻ Em Cetaphil Baby Shampoo 200ml', 'LSP006', 'TH0025', N'Chai', 178000, null, 'SP044.png', ' ', 'NCC010', 'NV0035', '2025-02-20');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0045', N'Sáp Ngăn Mùi Nivea Men Phân Tử Bạc 50ml', 'LSP007', 'TH0017', N'Thanh', 81000, null, 'SP045.png', ' ', 'NCC005', 'NV0035', '2025-02-20');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0046', N'Xịt Ngăn Mùi Nam Than Đen Hoạt Tính Nivea 150ml', 'LSP007', 'TH0017', N'Chai', 129000, null, 'SP046.png', ' ', 'NCC005', 'NV0035', '2025-02-20');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0047', N'Dao Cạo Gillette Blue II Plus', 'LSP007', 'TH0014', N'Tuýp', 52000, null, 'SP047.png', ' ', 'NCC004', 'NV0035', '2025-02-20');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0048', N'Sáp Thơm Ambi Pur Hương Downy Huyền Bí 180g', 'LSP008', 'TH0015', N'Hộp', 68000, null, 'SP048.png', ' ', 'NCC004', 'NV0035', '2025-02-20');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0049', N'Nước Xịt Vải Downy Hương Đam Mê 370ml', 'LSP008', 'TH0015', N'Chai', 86000, null, 'SP049.png', ' ', 'NCC004', 'NV0035', '2025-02-20');

insert into tblSanPham (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat) 
values ('SP0050', N'Nước Xả Vải Downy Hương Cotton Thuần Khiết 1L', 'LSP008', 'TH0015', N'Túi', 79000, null, 'SP050.png', ' ', 'NCC004', 'NV0035', '2025-02-20');


									--------------------------------------------------------------------
															-- BẢNG KHUYẾN MÃI --
                                    --------------------------------------------------------------------
-- Dữ liệu khuyến mãi
insert into tblKhuyenMai (MaKhuyenMai, TenKhuyenMai, GiaTri, MoTa, GiaTriDonHang, LoaiDonHangApDung, HangThanhVienApDung, 
            ApDungThangSinhNhat, ApDungKhachHangMoi, NgayBatDau, NgayKetThuc, SoLanSuDungToiDa, MaNhanVien)
values	('KM0001', N'Mừng thành viên tháng sinh nhật', 50000, N'Tặng voucher 50K đơn 100K mừng tháng sinh nhật', 100000, N'Tất cả', N'Tất cả', 1, 0, null, null, 1, 'NV0004'),
		('KM0002', N'Khách hàng mới', 0.12, N'Giảm 12% tối đa 100K cho đơn hàng đầu tiên trị giá từ 800K', 800000, N'Tất cả', N'Tất cả', 0, 1, '2025-09-01', '2025-12-31', 1, 'NV0004'),
		('KM0003', N'Khách hàng mới - online', 0.1, N'Giảm 10% cho đơn hàng online đầu tiên', null, N'Trực tuyến', N'Tất cả', 0, 1, '2025-09-01', '2025-12-31', 1, 'NV0004'),
		('KM0004', N'Giảm 40K đơn từ 999K', 40000, null, 999000, N'Tất cả', N'Tất cả', 0, 0, '2025-09-12', '2025-12-03', 5, 'NV0013'),
		('KM0005', N'Giảm 20K đơn từ 599K', 20000, null, 599000, N'Tất cả', N'Tất cả', 0, 0, '2025-09-12', '2025-12-03', 5, 'NV0013'); 

----------------------------------------------------------------------
-- TRIGGER BẢNG KHUYẾN MÃI
----------------------------------------------------------------------

CREATE TRIGGER trg_NhapKhuyenMai_InsertUpdate
ON tblKhuyenMai
INSTEAD OF INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    ----------------------------------------------------------------------
    -- Lấy dữ liệu
    ----------------------------------------------------------------------
    DECLARE 
        @Ten NVARCHAR(100),
        @MoTa NVARCHAR(255),
        @GiaTri DECIMAL(18,2),
        @GiaTriDH DECIMAL(18,2),
        @LoaiDH NVARCHAR(20),
        @HangTV NVARCHAR(20),
        @ThangSN BIT,
        @KhachMoi BIT,
        @MaSP NCHAR(6),
        @MaLoaiSP NCHAR(6),
        @MaTH NCHAR(6),
        @NgayBD DATE,
        @NgayKT DATE,
        @SoLan INT,
        @TrangThai BIT,
        @MaNV NCHAR(6),
        @MaKM NCHAR(6);

    SELECT 
        @Ten = TenKhuyenMai,
        @MoTa = MoTa,
        @GiaTri = GiaTri,
        @GiaTriDH = GiaTriDonHang,
        @LoaiDH = LoaiDonHangApDung,
        @HangTV = HangThanhVienApDung,
        @ThangSN = ApDungThangSinhNhat,
        @KhachMoi = ApDungKhachHangMoi,
        @MaSP = MaSanPham,
        @MaLoaiSP = MaLoaiSanPham,
        @MaTH = MaThuongHieu,
        @NgayBD = NgayBatDau,
        @NgayKT = NgayKetThuc,
        @SoLan = SoLanSuDungToiDa,
        @TrangThai = TrangThaiKhuyenMai,
        @MaNV = MaNhanVien,
        @MaKM = MaKhuyenMai
    FROM inserted;

    ----------------------------------------------------------------------
    -- 1. VALIDATE TÊN KHUYẾN MÃI
    ----------------------------------------------------------------------
    IF @Ten LIKE '%[^A-Za-z0-9 ]%'
    BEGIN
        RAISERROR(N'Tên chỉ được chứa chữ, số và khoảng trắng.', 16, 1);
        RETURN;
    END;

    IF @Ten LIKE '[0-9]%'
    BEGIN
        RAISERROR(N'Tên khuyến mãi không được bắt đầu bằng số.', 16, 1);
        RETURN;
    END;

    -- Viết hoa chữ cái đầu toàn chuỗi
    DECLARE @TenFormatted NVARCHAR(100);
    SET @TenFormatted = UPPER(LEFT(@Ten,1)) + LOWER(SUBSTRING(@Ten,2,LEN(@Ten)));

    ----------------------------------------------------------------------
    -- 2. VALIDATE MÔ TẢ
    ----------------------------------------------------------------------
    IF @MoTa IS NOT NULL AND
       @MoTa LIKE '%[^A-Za-z0-9 .,!?;:+\-*/₫$€()\[\]{}_\r\n]%'
    BEGIN
        RAISERROR(N'Mô tả chứa ký tự không hợp lệ.', 16, 1);
        RETURN;
    END;

    DECLARE @MoTaFormatted NVARCHAR(255);
    IF @MoTa IS NOT NULL
        SET @MoTaFormatted = UPPER(LEFT(@MoTa,1)) + SUBSTRING(@MoTa,2,LEN(@MoTa));
    ELSE 
        SET @MoTaFormatted = NULL;

    ----------------------------------------------------------------------
    -- 3. VALIDATE NGÀY
    ----------------------------------------------------------------------
    IF @NgayBD IS NOT NULL AND @NgayKT IS NOT NULL AND @NgayBD > @NgayKT
    BEGIN
        RAISERROR(N'Ngày bắt đầu phải trước ngày kết thúc.', 16, 1);
        RETURN;
    END;

    ----------------------------------------------------------------------
    -- 4. INSERT – Sinh mã KMxxxx
    ----------------------------------------------------------------------
    IF NOT EXISTS (
        SELECT 1 FROM tblKhuyenMai KM JOIN inserted i ON KM.MaKhuyenMai = i.MaKhuyenMai
    )
    BEGIN
        DECLARE @NewID NCHAR(6);
        DECLARE @MaxID INT;

        SELECT @MaxID = MAX(CAST(SUBSTRING(MaKhuyenMai, 3, 4) AS INT))
        FROM tblKhuyenMai;

        SET @MaxID = ISNULL(@MaxID, 0) + 1;
        SET @NewID = 'KM' + RIGHT('0000' + CAST(@MaxID AS VARCHAR(4)), 4);

        INSERT INTO tblKhuyenMai (
            MaKhuyenMai, TenKhuyenMai, GiaTri, MoTa, GiaTriDonHang,
            LoaiDonHangApDung, HangThanhVienApDung, 
            ApDungThangSinhNhat, ApDungKhachHangMoi,
            MaSanPham, MaLoaiSanPham, MaThuongHieu,
            NgayBatDau, NgayKetThuc, SoLanSuDungToiDa,
            TrangThaiKhuyenMai, MaNhanVien
        )
        SELECT 
            @NewID, @TenFormatted, @GiaTri, @MoTaFormatted, @GiaTriDH,
            @LoaiDH, @HangTV,
            @ThangSN, @KhachMoi,
            @MaSP, @MaLoaiSP, @MaTH,
            @NgayBD, @NgayKT, @SoLan,
            @TrangThai, @MaNV;

        RETURN;
    END;

    ----------------------------------------------------------------------
    -- 5. UPDATE – chỉ cho cập nhật các field không phải khóa chính
    ----------------------------------------------------------------------
    UPDATE KM
    SET 
        TenKhuyenMai = @TenFormatted,
        GiaTri = @GiaTri,
        MoTa = @MoTaFormatted,
        GiaTriDonHang = @GiaTriDH,
        LoaiDonHangApDung = @LoaiDH,
        HangThanhVienApDung = @HangTV,
        ApDungThangSinhNhat = @ThangSN,
        ApDungKhachHangMoi = @KhachMoi,
        MaSanPham = @MaSP,
        MaLoaiSanPham = @MaLoaiSP,
        MaThuongHieu = @MaTH,
        NgayBatDau = @NgayBD,
        NgayKetThuc = @NgayKT,
        SoLanSuDungToiDa = @SoLan,
        TrangThaiKhuyenMai = @TrangThai,
        MaNhanVien = @MaNV
    FROM tblKhuyenMai KM
    JOIN inserted i ON KM.MaKhuyenMai = i.MaKhuyenMai;

END;
GO

									--------------------------------------------------------------------
															-- BẢNG ĐƠN HÀNG --
                                    --------------------------------------------------------------------

-- Dữ liệu đơn hàng
insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0001', 'KH0010', 'NV0049', N'Trực tuyến', '2025-10-25', N'Giao hàng thành công', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0002', 'KH0012', 'NV0049', N'Trực tuyến', '2025-10-25', N'Giao hàng thành công', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0003', 'KH0014', 'NV0049', N'Trực tuyến', '2025-10-25', N'Giao hàng thành công', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0004', 'KH0001', 'NV0005', N'Tại cửa hàng', '2025-10-25', N'Giao hàng thành công', 0, 0, 'CN01');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0005', 'KH0002', 'NV0005', N'Tại cửa hàng', '2025-10-26', N'Giao hàng thành công', 0, 0, 'CN01');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0006', 'KH0005', 'NV0005', N'Tại cửa hàng', '2025-10-26', N'Giao hàng thành công', 0, 0, 'CN01');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0007', 'KH0006', 'NV0005', N'Tại cửa hàng', '2025-10-26', N'Giao hàng thành công', 0, 0, 'CN01');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0008', 'KH0007', 'NV0005', N'Tại cửa hàng', '2025-10-26', N'Giao hàng thành công', 0, 0, 'CN01');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0009', 'KH0008', 'NV0005', N'Tại cửa hàng', '2025-10-26', N'Giao hàng thành công', 0, 0, 'CN01');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0010', 'KH0043', 'NV0014', N'Tại cửa hàng', '2025-10-26', N'Giao hàng thành công', 0, 0, 'CN02');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0011', 'KH0050', 'NV0014', N'Tại cửa hàng', '2025-10-26', N'Giao hàng thành công', 0, 0, 'CN02');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0012', 'KH0051', 'NV0014', N'Tại cửa hàng', '2025-10-27', N'Giao hàng thành công', 0, 0, 'CN02');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0013', 'KH0052', 'NV0014', N'Tại cửa hàng', '2025-10-27', N'Giao hàng thành công', 0, 0, 'CN02');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0014', 'KH0053', 'NV0014', N'Tại cửa hàng', '2025-10-28', N'Giao hàng thành công', 0, 0, 'CN02');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0015', 'KH0055', 'NV0014', N'Tại cửa hàng', '2025-10-28', N'Giao hàng thành công', 0, 0, 'CN02');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0016', 'KH0021', 'NV0022', N'Tại cửa hàng', '2025-10-28', N'Giao hàng thành công', 0, 0, 'CN03');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0017', 'KH0022', 'NV0022', N'Tại cửa hàng', '2025-10-28', N'Giao hàng thành công', 0, 0, 'CN03');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0018', 'KH0025', 'NV0022', N'Tại cửa hàng', '2025-10-29', N'Giao hàng thành công', 0, 0, 'CN03');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0019', 'KH0042', 'NV0022', N'Tại cửa hàng', '2025-10-29', N'Giao hàng thành công', 0, 0, 'CN03');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0020', 'KH0049', 'NV0030', N'Tại cửa hàng', '2025-10-29', N'Giao hàng thành công', 0, 0, 'CN05');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0021', 'KH0056', 'NV0030', N'Tại cửa hàng', '2025-10-29', N'Giao hàng thành công', 0, 0, 'CN05');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0022', 'KH0058', 'NV0030', N'Tại cửa hàng', '2025-10-29', N'Giao hàng thành công', 0, 0, 'CN05');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0023', 'KH0015', 'NV0038', N'Tại cửa hàng', '2025-10-29', N'Giao hàng thành công', 0, 0, 'CN04');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0024', 'KH0016', 'NV0038', N'Tại cửa hàng', '2025-10-29', N'Giao hàng thành công', 0, 0, 'CN04');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0025', 'KH0019', 'NV0038', N'Tại cửa hàng', '2025-10-30', N'Giao hàng thành công', 0, 0, 'CN04');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0026', 'KH0026', 'NV0049', N'Trực tuyến', '2025-10-30', N'Đang giao hàng', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0027', 'KH0028', 'NV0049', N'Trực tuyến', '2025-11-01', N'Đang giao hàng', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0028', 'KH0031', 'NV0049', N'Trực tuyến', '2025-11-01', N'Đang giao hàng', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0029', 'KH0037', 'NV0049', N'Trực tuyến', '2025-11-01', N'Đang giao hàng', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0030', 'KH0038', 'NV0049', N'Trực tuyến', '2025-11-01', N'Đang giao hàng', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0031', 'KH0032', 'NV0049', N'Trực tuyến', '2025-11-01', N'Đã lấy hàng', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0032', 'KH0034', 'NV0049', N'Trực tuyến', '2025-11-01', N'Đã lấy hàng', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0033', 'KH0039', 'NV0049', N'Trực tuyến', '2025-11-01', N'Đã lấy hàng', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0034', 'KH0040', 'NV0049', N'Trực tuyến', '2025-11-02', N'Đã lấy hàng', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0035', 'KH0041', 'NV0049', N'Trực tuyến', '2025-11-02', N'Đã lấy hàng', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0036', 'KH0035', 'NV0048', N'Trực tuyến', '2025-11-02', N'Chờ lấy hàng', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0037', 'KH0045', 'NV0048', N'Trực tuyến', '2025-11-02', N'Chờ lấy hàng', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0038', 'KH0046', 'NV0048', N'Trực tuyến', '2025-11-02', N'Chờ lấy hàng', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0039', 'KH0047', 'NV0048', N'Trực tuyến', '2025-11-02', N'Chờ lấy hàng', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0040', 'KH0048', 'NV0048', N'Trực tuyến', '2025-11-02', N'Chờ lấy hàng', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0041', 'KH0027', 'NV0049', N'Trực tuyến', '2025-11-02', N'Đã xác nhận', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0042', 'KH0029', 'NV0049', N'Trực tuyến', '2025-11-03', N'Đã xác nhận', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0043', 'KH0012', 'NV0049', N'Trực tuyến', '2025-11-03', N'Đã xác nhận', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0044', 'KH0014', 'NV0049', N'Trực tuyến', '2025-11-03', N'Đã xác nhận', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0045', 'KH0036', 'NV0049', N'Trực tuyến', '2025-11-03', N'Đã xác nhận', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0046', 'KH0010', 'NV0047', N'Trực tuyến', '2025-11-03', N'Chờ xác nhận', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0047', 'KH0014', 'NV0047', N'Trực tuyến', '2025-11-03', N'Chờ xác nhận', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0048', 'KH0033', 'NV0047', N'Trực tuyến', '2025-11-03', N'Chờ xác nhận', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0049', 'KH0040', 'NV0047', N'Trực tuyến', '2025-11-03', N'Chờ xác nhận', 0, 0, 'CN06');

insert into tblDonHang (MaDonHang, MaKhachHang, MaNhanVien, LoaiDonHang, NgayDatHang, TrangThaiDonHang, TongTienHang, ThanhTien, MaChiNhanh)
values	('DH0050', 'KH0048', 'NV0047', N'Trực tuyến', '2025-11-03', N'Chờ xác nhận', 0, 0, 'CN06');

---------------------------------------------------------------------	
-- Tạo hàm chọn mã khuyến mãi
---------------------------------------------------------------------
CREATE FUNCTION fn_ChonKhuyenMaiTotNhat (@MaDonHang nchar(6))
RETURNS nchar(6)
AS
BEGIN
    DECLARE @MaKH nchar(6),
            @LoaiDH nvarchar(20),
            @NgayDH datetime,
            @TongTien decimal(18,2),
            @HangTV nvarchar(20),
            @MaKQ nchar(6);

    -- Lấy thông tin đơn hàng + khách hàng
    SELECT 
        @MaKH = dh.MaKhachHang,
        @LoaiDH = dh.LoaiDonHang,
        @NgayDH = dh.NgayDatHang,
        @TongTien = dh.TongTienHang
    FROM tblDonHang dh
    WHERE dh.MaDonHang = @MaDonHang;

    SELECT @HangTV = k.HangThanhVien
    FROM tblKhachHang k
    WHERE k.MaKhachHang = @MaKH;

    ---------------------------------------------------------------------
    -- Tập mã khuyến mãi hợp lệ (lọc theo từng điều kiện)
    ---------------------------------------------------------------------
    ;WITH KM AS (
        SELECT *,
               -- Đếm số điều kiện đang được dùng (cột không null)
               (CASE WHEN GiaTriDonHang IS NOT NULL THEN 1 ELSE 0 END
              + CASE WHEN LoaiDonHangApDung IS NOT NULL THEN 1 ELSE 0 END
              + CASE WHEN HangThanhVienApDung IS NOT NULL THEN 1 ELSE 0 END
              + CASE WHEN ApDungThangSinhNhat = 1 THEN 1 ELSE 0 END
              + CASE WHEN ApDungKhachHangMoi = 1 THEN 1 ELSE 0 END
              + CASE WHEN MaSanPham IS NOT NULL THEN 1 ELSE 0 END
              + CASE WHEN MaLoaiSanPham IS NOT NULL THEN 1 ELSE 0 END
              + CASE WHEN MaThuongHieu IS NOT NULL THEN 1 ELSE 0 END
              + CASE WHEN NgayBatDau IS NOT NULL THEN 1 ELSE 0 END) AS SoDieuKien
        FROM tblKhuyenMai
        WHERE TrangThaiKhuyenMai = 1
    ),
    HopLe AS (
        SELECT KM.*
        FROM KM
        WHERE
            -- 1. Giá trị đơn hàng
            (KM.GiaTriDonHang IS NULL OR @TongTien >= KM.GiaTriDonHang)

            -- 2. Loại đơn hàng
            AND (KM.LoaiDonHangApDung IS NULL 
                 OR KM.LoaiDonHangApDung = N'Tất cả'
                 OR KM.LoaiDonHangApDung = @LoaiDH)

            -- 3. Hạng thành viên
            AND (KM.HangThanhVienApDung IS NULL
                 OR KM.HangThanhVienApDung = N'Tất cả'
                 OR KM.HangThanhVienApDung = @HangTV)

            -- 4. Sinh nhật tháng
            AND (KM.ApDungThangSinhNhat = 0
                 OR MONTH(@NgayDH) = (SELECT MONTH(NgaySinh) 
                                      FROM tblKhachHang WHERE MaKhachHang = @MaKH))

            -- 5. Khách hàng mới
            AND (KM.ApDungKhachHangMoi = 0
                 OR NOT EXISTS (SELECT 1 FROM tblDonHang WHERE MaKhachHang = @MaKH AND MaDonHang <> @MaDonHang))

            -- 6. Sản phẩm
            AND (
                    KM.MaSanPham IS NULL 
                    OR EXISTS (
                        SELECT 1 
                        FROM tblChiTietDonHang 
                        WHERE MaDonHang = @MaDonHang AND MaSanPham = KM.MaSanPham
                    )
                )

            -- 7. Loại sản phẩm
            AND (
                    KM.MaLoaiSanPham IS NULL
                    OR EXISTS (
                        SELECT 1
                        FROM tblChiTietDonHang ct
                        JOIN tblSanPham sp ON sp.MaSanPham = ct.MaSanPham
                        WHERE ct.MaDonHang = @MaDonHang AND sp.MaLoaiSanPham = KM.MaLoaiSanPham
                    )
                )

            -- 8. Thương hiệu
            AND (
                    KM.MaThuongHieu IS NULL
                    OR EXISTS (
                        SELECT 1
                        FROM tblChiTietDonHang ct
                        JOIN tblSanPham sp ON sp.MaSanPham = ct.MaSanPham
                        WHERE ct.MaDonHang = @MaDonHang AND sp.MaThuongHieu = KM.MaThuongHieu
                    )
                )

            -- 9. Ngày áp dụng
            AND (@NgayDH BETWEEN KM.NgayBatDau AND KM.NgayKetThuc)
    )

    ---------------------------------------------------------------------
    -- Chọn mã tốt nhất:
    -- Ưu tiên:  
    --   1) Nhiều điều kiện nhất  
    --   2) Giá trị cao nhất

    SELECT TOP 1 @MaKQ = MaKhuyenMai
    FROM HopLe
    ORDER BY SoDieuKien DESC, GiaTri DESC;

    RETURN @MaKQ;
END;
GO

---------------------------------------------------------------------
-- Tự động gán mã KM + tính điểm + thành tiền
---------------------------------------------------------------------
CREATE TRIGGER trg_XuLyDonHang
ON tblDonHang
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaDH nchar(6);

    SELECT @MaDH = MaDonHang FROM inserted;

    -- 1. Gọi function chọn mã khuyến mãi
    DECLARE @MaKM nchar(6) = dbo.fn_ChonKhuyenMaiTotNhat(@MaDH);

    UPDATE tblDonHang
    SET MaKhuyenMai = @MaKM
    WHERE MaDonHang = @MaDH;

    -- 2. Lấy giá trị KM   
    DECLARE @GiaTriKM decimal(18,2) = (
        SELECT GiaTri FROM tblKhuyenMai WHERE MaKhuyenMai = @MaKM
    );

    IF @GiaTriKM IS NULL SET @GiaTriKM = 0;

    ---------------------------------------------------------------------
    -- 3. Tính giá trị điểm đổi
    ---------------------------------------------------------------------
    DECLARE @DiemSuDung int = (SELECT DiemSuDung FROM tblDonHang WHERE MaDonHang = @MaDH);
    DECLARE @GiaTriDiem decimal(18,2) = @DiemSuDung / 100 * 5000;

    ---------------------------------------------------------------------
    -- 4. Thành tiền
    ---------------------------------------------------------------------
    UPDATE tblDonHang
    SET ThanhTien = TongTienHang - @GiaTriKM - @GiaTriDiem
    WHERE MaDonHang = @MaDH;

    ---------------------------------------------------------------------
    -- 5. Kiểm tra loại đơn hàng
    ---------------------------------------------------------------------
    IF EXISTS (
        SELECT 1 FROM tblDonHang 
        WHERE MaDonHang = @MaDH
          AND LoaiDonHang = N'Tại cửa hàng'
          AND TrangThaiDonHang <> N'Giao hàng thành công'
    )
    BEGIN
        RAISERROR(N'Đơn hàng tại cửa hàng phải có trạng thái: Giao hàng thành công!', 16, 1);
        ROLLBACK;
        RETURN;
    END;
END;
GO

---------------------------------------------------------------------
-- Cập nhật thanh toán theo cập nhật đơn hàng
---------------------------------------------------------------------
---------------------------------------------------------------------
-- Cập nhật thanh toán theo cập nhật đơn hàng (set-based, an toàn)
---------------------------------------------------------------------
CREATE OR ALTER TRIGGER trg_CapNhatThanhToanTheoDonHang
ON tblDonHang
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM inserted) RETURN;

    IF TRIGGER_NESTLEVEL() > 1 RETURN;

    -- Lấy các payment liên quan tới các đơn vừa thay đổi
    DECLARE @T TABLE (
        MaDonHang nchar(6),
        MaThanhToan nchar(6),
        LoaiDonHang nvarchar(20),
        PhuongThucThanhToan nvarchar(50),
        TrangThaiDonHangMoi nvarchar(50),
        NgayDatHang datetime,
        ThanhTien decimal(18,2)
    );

    INSERT INTO @T(MaDonHang, MaThanhToan, LoaiDonHang, PhuongThucThanhToan, TrangThaiDonHangMoi, NgayDatHang, ThanhTien)
    SELECT dh.MaDonHang, tt.MaThanhToan, dh.LoaiDonHang, tt.PhuongThucThanhToan,
           dh.TrangThaiDonHang, dh.NgayDatHang, dh.ThanhTien
    FROM inserted dh
    JOIN tblThanhToan tt ON dh.MaDonHang = tt.MaDonHang;

    -- Cập nhật payment theo logic (set TongTien = ThanhTien)
    UPDATE tt
    SET 
        tt.TongTien = t.ThanhTien,
        tt.TrangThaiThanhToan = CASE 
            WHEN t.LoaiDonHang = N'Tại cửa hàng' THEN 1
            WHEN t.LoaiDonHang = N'Trực tuyến' AND t.PhuongThucThanhToan <> N'Tiền mặt khi giao hàng' THEN 1
            WHEN t.LoaiDonHang = N'Trực tuyến' AND t.PhuongThucThanhToan = N'Tiền mặt khi giao hàng'
                 AND t.TrangThaiDonHangMoi = N'Giao hàng thành công' THEN 1
            ELSE 0
        END,
        tt.NgayThanhToan = CASE 
            WHEN t.LoaiDonHang = N'Tại cửa hàng' THEN t.NgayDatHang
            WHEN t.LoaiDonHang = N'Trực tuyến' AND t.PhuongThucThanhToan <> N'Tiền mặt khi giao hàng' THEN GETDATE()
            WHEN t.LoaiDonHang = N'Trực tuyến' AND t.PhuongThucThanhToan = N'Tiền mặt khi giao hàng'
                 AND t.TrangThaiDonHangMoi = N'Giao hàng thành công' THEN GETDATE()
            ELSE tt.NgayThanhToan
        END
    FROM tblThanhToan tt
    JOIN @T t ON tt.MaThanhToan = t.MaThanhToan
             AND tt.MaDonHang = t.MaDonHang;
END
GO



---------------------------------------------------------------------	
-- Cập nhật điểm tích lũy sau khi đơn hàng thành công
---------------------------------------------------------------------
CREATE TRIGGER trg_CapNhatDiemKhachHang
ON tblDonHang
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Tránh đệ quy trigger
    IF TRIGGER_NESTLEVEL() > 1 RETURN;

    DECLARE @BangThayDoi TABLE (
        MaDonHang nchar(6),
        MaKhachHang nchar(6),
        ThanhTien decimal(18,2),
        TrangThaiDonHang nvarchar(50)
    );

    INSERT INTO @BangThayDoi
    SELECT COALESCE(i.MaDonHang, d.MaDonHang),
           COALESCE(i.MaKhachHang, d.MaKhachHang),
           COALESCE(i.ThanhTien, d.ThanhTien,0),
           COALESCE(i.TrangThaiDonHang, d.TrangThaiDonHang, N'')
    FROM inserted i
    FULL OUTER JOIN deleted d ON i.MaDonHang = d.MaDonHang;

    -- Cập nhật điểm tích luỹ trên tblKhachHang
    -- Trừ điểm cũ nếu đơn hàng cũ thành công
    UPDATE kh
	SET DiemTichLuy = kh.DiemTichLuy - FLOOR(b.ThanhTien/10000)
	FROM tblKhachHang kh
	JOIN @BangThayDoi b ON kh.MaKhachHang = b.MaKhachHang
	WHERE kh.LoaiKhachHang = N'Thành viên'
	  AND EXISTS (
			SELECT 1 FROM deleted d 
			WHERE d.MaDonHang = b.MaDonHang 
			  AND d.TrangThaiDonHang = N'Giao hàng thành công'
	  );

    -- Cộng điểm mới nếu đơn hàng mới thành công
	UPDATE kh
	SET DiemTichLuy = kh.DiemTichLuy + FLOOR(b.ThanhTien/10000)
	FROM tblKhachHang kh
	JOIN @BangThayDoi b ON kh.MaKhachHang = b.MaKhachHang
	WHERE kh.LoaiKhachHang = N'Thành viên'
	  AND b.TrangThaiDonHang = N'Giao hàng thành công';

    -- Cập nhật chi tiết điểm tích luỹ
    MERGE tblChiTietDiemTichLuy AS tgt
	USING (
    SELECT MaKhachHang, MaDonHang, FLOOR(ThanhTien/10000) AS DiemMoi
    FROM @BangThayDoi
    WHERE TrangThaiDonHang = N'Giao hàng thành công'
      AND MaKhachHang IN (SELECT MaKhachHang FROM tblKhachHang WHERE LoaiKhachHang = N'Thành viên')
	) AS src
	ON tgt.MaKhachHang = src.MaKhachHang AND tgt.MaDonHang = src.MaDonHang
	WHEN MATCHED THEN
		UPDATE SET DiemTichLuy = src.DiemMoi,
				   NgayHetHan = DATEFROMPARTS(YEAR(GETDATE())+2,12,31)
	WHEN NOT MATCHED THEN
		INSERT (MaKhachHang, MaDonHang, DiemDaSuDung, DiemTichLuy, NgayHetHan)
		VALUES (src.MaKhachHang, src.MaDonHang, 0, src.DiemMoi, DATEFROMPARTS(YEAR(GETDATE())+2,12,31));

    -- Xóa chi tiết điểm nếu đơn hàng không thành công
    DELETE tgt
	FROM tblChiTietDiemTichLuy tgt
	JOIN @BangThayDoi b ON tgt.MaKhachHang = b.MaKhachHang AND tgt.MaDonHang = b.MaDonHang
	JOIN tblKhachHang kh ON kh.MaKhachHang = b.MaKhachHang
	WHERE kh.LoaiKhachHang = N'Thành viên'
	  AND b.TrangThaiDonHang <> N'Giao hàng thành công';
END
GO

---------------------------------------------------------------------
-- Cập nhật hạng thành viên 
---------------------------------------------------------------------
CREATE TRIGGER trg_CapNhatHangThanhVien
ON tblDonHang
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Chống đệ quy
    IF TRIGGER_NESTLEVEL() > 1 RETURN;


    -------------------------------------------------------------------
    -- 1. Chỉ lấy những đơn hàng có thay đổi trạng thái thực sự
    -------------------------------------------------------------------
    DECLARE @ThayDoi TABLE(
        MaKhachHang nchar(6)
    );

    INSERT INTO @ThayDoi (MaKhachHang)
    SELECT DISTINCT i.MaKhachHang
    FROM inserted i
    JOIN deleted d ON i.MaDonHang = d.MaDonHang
    WHERE ISNULL(i.TrangThaiDonHang,'') <> ISNULL(d.TrangThaiDonHang,'');


    -- Không có thay đổi trạng thái → Không tính lại hạng
    IF NOT EXISTS (SELECT 1 FROM @ThayDoi) RETURN;


    -------------------------------------------------------------------
    -- 2. Chỉ cập nhật cho khách hàng "Thành viên"
    -------------------------------------------------------------------
    UPDATE kh
    SET HangThanhVien =
        CASE 
            WHEN tong.TongThanhTien >= 10000000 THEN N'Watsons Club Elite'
            ELSE N'Watsons Club'
        END,
        NgayCapNhat = GETDATE()
    FROM tblKhachHang kh
    JOIN @ThayDoi td ON td.MaKhachHang = kh.MaKhachHang
    CROSS APPLY (
        SELECT SUM(ThanhTien) AS TongThanhTien
        FROM tblDonHang dh
        WHERE dh.MaKhachHang = kh.MaKhachHang
          AND dh.TrangThaiDonHang = N'Giao hàng thành công'
          AND dh.NgayDatHang >= DATEADD(YEAR, -1, GETDATE())
    ) tong
    WHERE kh.LoaiKhachHang = N'Thành viên';
END
GO


---------------------------------------------------------------------
-- Cập nhật mã khuyến mãi của khách hàng
---------------------------------------------------------------------
CREATE TRIGGER trg_CapNhatMaKMCuaKH
ON tblDonHang
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    ---------------------------------------------------------------------
    -- 0. Tránh đệ quy trigger

    IF TRIGGER_NESTLEVEL() > 1 
        RETURN;
		    ------------------------------------------------------------------
    -- 1) Xử lý INSERT: Đơn hàng mới + thành công -> tăng số lần dùng

    INSERT INTO tblMaKMCuaKH (MaKhachHang, MaKhuyenMai, SoLanDaSuDung)
    SELECT i.MaKhachHang, i.MaKhuyenMai, 1
    FROM inserted i
    WHERE i.MaKhuyenMai IS NOT NULL
      AND i.TrangThaiDonHang = N'Giao hàng thành công'
      AND NOT EXISTS (
            SELECT 1 FROM tblMaKMCuaKH 
            WHERE MaKhachHang = i.MaKhachHang AND MaKhuyenMai = i.MaKhuyenMai
      );

    UPDATE tgt
    SET SoLanDaSuDung = SoLanDaSuDung + 1
    FROM tblMaKMCuaKH tgt
    JOIN inserted i 
        ON tgt.MaKhachHang = i.MaKhachHang
       AND tgt.MaKhuyenMai = i.MaKhuyenMai
    WHERE i.MaKhuyenMai IS NOT NULL
      AND i.TrangThaiDonHang = N'Giao hàng thành công';

	    ------------------------------------------------------------------
    -- 2) Xử lý UPDATE trạng thái: thành công -> không thành công

    UPDATE tgt
    SET SoLanDaSuDung = CASE WHEN SoLanDaSuDung>0 THEN SoLanDaSuDung-1 ELSE 0 END
    FROM tblMaKMCuaKH tgt
    JOIN inserted i ON tgt.MaKhachHang = i.MaKhachHang AND tgt.MaKhuyenMai = i.MaKhuyenMai
    JOIN deleted d  ON d.MaDonHang = i.MaDonHang
    WHERE i.MaKhuyenMai IS NOT NULL
      AND d.TrangThaiDonHang = N'Giao hàng thành công'
      AND i.TrangThaiDonHang <> N'Giao hàng thành công';

    ------------------------------------------------------------------
    -- 3) Xử lý DELETE: nếu xoá đơn thành công → giảm số lần dùng

    UPDATE tgt
    SET SoLanDaSuDung = CASE WHEN SoLanDaSuDung>0 THEN SoLanDaSuDung-1 ELSE 0 END
    FROM tblMaKMCuaKH tgt
    JOIN deleted d ON tgt.MaKhachHang = d.MaKhachHang AND tgt.MaKhuyenMai = d.MaKhuyenMai
    WHERE d.MaKhuyenMai IS NOT NULL
      AND d.TrangThaiDonHang = N'Giao hàng thành công';
END
GO


EXEC sp_settriggerorder 
    @triggername = 'trg_XuLyDonHang', 
    @order = 'First', 
    @stmttype = 'UPDATE';

EXEC sp_settriggerorder 
    @triggername = 'trg_CapNhatThanhToanTheoDonHang', 
    @order = 'Last', 
    @stmttype = 'UPDATE';

EXEC sp_settriggerorder 
    @triggername = 'trg_CapNhatDiemKhachHang',
    @order = 'None',
    @stmttype = 'UPDATE';

EXEC sp_settriggerorder 
    @triggername = 'trg_CapNhatMaKMCuaKH',
    @order = 'None',
    @stmttype = 'UPDATE';

	EXEC sp_settriggerorder 
    @triggername = 'trg_CapNhatHangThanhVien',
    @order = 'None',
    @stmttype = 'UPDATE';

---------------------------------------------------------------------
-- TRIGGER BẢNG ĐƠN HÀNG (MỚI)
---------------------------------------------------------------------
---------------------------------------------------------------------
-- TRIGGER INSTEAD OF DELETE: XÓA ĐƠN HÀNG AN TOÀN
---------------------------------------------------------------------
CREATE OR ALTER TRIGGER trg_XoaDonHang
ON tblDonHang
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Xóa chi tiết điểm tích lũy
    DELETE FROM tblChiTietDiemTichLuy
    WHERE MaDonHang IN (SELECT MaDonHang FROM deleted);

    -- 2. Xóa chi tiết đơn hàng
    DELETE FROM tblChiTietDonHang
    WHERE MaDonHang IN (SELECT MaDonHang FROM deleted);

    -- 3. Xóa thanh toán
    DELETE FROM tblThanhToan
    WHERE MaDonHang IN (SELECT MaDonHang FROM deleted);

    -- 4. Cuối cùng xóa đơn hàng
    DELETE FROM tblDonHang
    WHERE MaDonHang IN (SELECT MaDonHang FROM deleted);
END
GO


---------------------------------------------------------------------
-- TRIGGER AFTER INSERT, UPDATE: CẬP NHẬT ĐƠN HÀNG SET-BASED
---------------------------------------------------------------------
CREATE OR ALTER TRIGGER trg_XuLyDonHang_SetBased
ON tblDonHang
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- chống đệ quy
    IF TRIGGER_NESTLEVEL() > 1 RETURN;

    /*============================================================
      CHẶN ROLLBACK KHI ĐÃ GIAO HÀNG THÀNH CÔNG
    ============================================================*/
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN deleted d ON i.MaDonHang = d.MaDonHang
        WHERE d.TrangThaiDonHang = N'Giao hàng thành công'
          AND i.TrangThaiDonHang <> d.TrangThaiDonHang
    )
        RETURN;

    /*============================================================
  0. CẬP NHẬT MÃ KHUYẾN MÃI
    ============================================================*/
    UPDATE dh
    SET MaKhuyenMai = i.MaKhuyenMai
    FROM tblDonHang dh
    JOIN inserted i ON dh.MaDonHang = i.MaDonHang;

    /*============================================================
      1. TÍNH THANH TIỀN (PHÂN BIỆT % / VND)
    ============================================================*/
    UPDATE dh
    SET ThanhTien =
          dh.TongTienHang
        - CASE 
            WHEN km.GiaTri < 1 THEN dh.TongTienHang * km.GiaTri
            ELSE ISNULL(km.GiaTri,0)
          END
        - ISNULL(diem.GiaTriDiem,0)
    FROM tblDonHang dh
    JOIN inserted i ON dh.MaDonHang = i.MaDonHang
    LEFT JOIN tblKhuyenMai km ON dh.MaKhuyenMai = km.MaKhuyenMai
    LEFT JOIN (
        SELECT MaDonHang,
               SUM(DiemSuDung / 100.0 * 5000) AS GiaTriDiem
        FROM inserted
        GROUP BY MaDonHang
    ) diem ON dh.MaDonHang = diem.MaDonHang;

    /*============================================================
      2. CẬP NHẬT THANH TOÁN (SIẾT ĐÚNG MỐC)
    ============================================================*/
    UPDATE tt
    SET
        tt.TongTien = dh.ThanhTien,
        tt.TrangThaiThanhToan = CASE 
            WHEN dh.LoaiDonHang = N'Tại cửa hàng'
                 AND dh.TrangThaiDonHang <> N'Đã xác nhận'
                THEN 1

            WHEN dh.LoaiDonHang = N'Trực tuyến'
                 AND tt.PhuongThucThanhToan <> N'Tiền mặt khi giao hàng'
                 AND dh.TrangThaiDonHang = N'Chờ lấy hàng'
                THEN 1

            WHEN dh.LoaiDonHang = N'Trực tuyến'
                 AND tt.PhuongThucThanhToan = N'Tiền mặt khi giao hàng'
                 AND dh.TrangThaiDonHang = N'Giao hàng thành công'
                THEN 1
            ELSE 0
        END,
        tt.NgayThanhToan = CASE 
            WHEN dh.LoaiDonHang = N'Tại cửa hàng'
                 AND dh.TrangThaiDonHang <> N'Đã xác nhận'
                THEN dh.NgayDatHang

            WHEN dh.LoaiDonHang = N'Trực tuyến'
                 AND tt.PhuongThucThanhToan <> N'Tiền mặt khi giao hàng'
                 AND dh.TrangThaiDonHang = N'Chờ lấy hàng'
                THEN GETDATE()

            WHEN dh.LoaiDonHang = N'Trực tuyến'
                 AND tt.PhuongThucThanhToan = N'Tiền mặt khi giao hàng'
                 AND dh.TrangThaiDonHang = N'Giao hàng thành công'
                THEN GETDATE()
            ELSE tt.NgayThanhToan
        END
    FROM tblThanhToan tt
    JOIN tblDonHang dh ON tt.MaDonHang = dh.MaDonHang
    JOIN inserted i ON dh.MaDonHang = i.MaDonHang;

    /*============================================================
      3. CỘNG / TRỪ ĐIỂM (CHỈ KHI GIAO HÀNG THÀNH CÔNG)
    ============================================================*/
    UPDATE kh
    SET DiemTichLuy = kh.DiemTichLuy - ISNULL(old.DiemCu,0)
    FROM tblKhachHang kh
    JOIN (
        SELECT d.MaKhachHang,
               SUM(FLOOR(d.ThanhTien / 10000)) AS DiemCu
        FROM deleted d
        WHERE d.TrangThaiDonHang = N'Giao hàng thành công'
        GROUP BY d.MaKhachHang
    ) old ON kh.MaKhachHang = old.MaKhachHang
    WHERE kh.LoaiKhachHang = N'Thành viên';

    UPDATE kh
    SET DiemTichLuy = kh.DiemTichLuy + ISNULL(newp.DiemMoi,0)
    FROM tblKhachHang kh
    JOIN (
        SELECT i.MaKhachHang,
               SUM(FLOOR(i.ThanhTien / 10000)) AS DiemMoi
        FROM inserted i
        WHERE i.TrangThaiDonHang = N'Giao hàng thành công'
        GROUP BY i.MaKhachHang
    ) newp ON kh.MaKhachHang = newp.MaKhachHang
    WHERE kh.LoaiKhachHang = N'Thành viên';

    /*============================================================
      4. CHI TIẾT ĐIỂM TÍCH LŨY
    ============================================================*/
    MERGE tblChiTietDiemTichLuy AS tgt
    USING (
        SELECT i.MaKhachHang,
               i.MaDonHang,
               FLOOR(i.ThanhTien / 10000) AS DiemMoi
        FROM inserted i
        JOIN tblKhachHang kh ON kh.MaKhachHang = i.MaKhachHang
        WHERE i.TrangThaiDonHang = N'Giao hàng thành công'
          AND kh.LoaiKhachHang = N'Thành viên'
    ) src
    ON tgt.MaKhachHang = src.MaKhachHang
       AND tgt.MaDonHang = src.MaDonHang
    WHEN MATCHED THEN
        UPDATE SET
            DiemTichLuy = src.DiemMoi,
            NgayHetHan = DATEFROMPARTS(YEAR(GETDATE()) + 2, 12, 31)
    WHEN NOT MATCHED THEN
        INSERT (MaKhachHang, MaDonHang, DiemDaSuDung, DiemTichLuy, NgayHetHan)
        VALUES (src.MaKhachHang, src.MaDonHang, 0, src.DiemMoi,
                DATEFROMPARTS(YEAR(GETDATE()) + 2, 12, 31));

    DELETE tgt
    FROM tblChiTietDiemTichLuy tgt
    JOIN inserted i
      ON tgt.MaKhachHang = i.MaKhachHang
     AND tgt.MaDonHang = i.MaDonHang
    WHERE i.TrangThaiDonHang <> N'Giao hàng thành công';

    /*============================================================
      5. CẬP NHẬT HẠNG
    ============================================================*/
    UPDATE kh
    SET HangThanhVien = CASE 
            WHEN tong.TongThanhTien >= 10000000
                 THEN N'Watsons Club Elite'
            ELSE N'Watsons Club'
        END,
        NgayCapNhat = GETDATE()
    FROM tblKhachHang kh
    CROSS APPLY (
        SELECT SUM(ThanhTien) AS TongThanhTien
        FROM tblDonHang dh
        WHERE dh.MaKhachHang = kh.MaKhachHang
          AND dh.TrangThaiDonHang = N'Giao hàng thành công'
          AND dh.NgayDatHang >= DATEADD(YEAR, -1, GETDATE())
    ) tong
    WHERE kh.LoaiKhachHang = N'Thành viên';

    /*============================================================
      6. KHUYẾN MÃI
    ============================================================*/
    INSERT INTO tblMaKMCuaKH (MaKhachHang, MaKhuyenMai, SoLanDaSuDung)
    SELECT i.MaKhachHang, i.MaKhuyenMai, 1
    FROM inserted i
    WHERE i.MaKhuyenMai IS NOT NULL
      AND i.TrangThaiDonHang = N'Giao hàng thành công'
      AND NOT EXISTS (
          SELECT 1
          FROM tblMaKMCuaKH t
          WHERE t.MaKhachHang = i.MaKhachHang
            AND t.MaKhuyenMai = i.MaKhuyenMai
      );

    UPDATE t
    SET SoLanDaSuDung = SoLanDaSuDung + 1
    FROM tblMaKMCuaKH t
    JOIN inserted i
      ON t.MaKhachHang = i.MaKhachHang
     AND t.MaKhuyenMai = i.MaKhuyenMai
    WHERE i.TrangThaiDonHang = N'Giao hàng thành công';
END
GO



									--------------------------------------------------------------------
														-- BẢNG CHI TIẾT ĐƠN HÀNG --
                                    --------------------------------------------------------------------

-- Dữ liệu Chi tiết đơn hàng
-- Dữ liệu Chi tiết đơn hàng (Tách)
insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0001', 'SP0006', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0002', 'SP0013', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0003', 'SP0035', 2, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0004', 'SP0021', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0005', 'SP0008', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0006', 'SP0011', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0007', 'SP0006', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0008', 'SP0022', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0009', 'SP0030', 2, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0010', 'SP0026', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values 		('DH0011', 'SP0047', 2, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values 		('DH0011', 'SP0034', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0012', 'SP0036', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0012', 'SP0038', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0013', 'SP0007', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0013', 'SP0005', 2, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0014', 'SP0014', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0014', 'SP0020', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0015', 'SP0023', 2, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0015', 'SP0024', 2, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0016', 'SP0026', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0016', 'SP0018', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0017', 'SP0032', 2, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0017', 'SP0049', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0018', 'SP0010', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0018', 'SP0013', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0019', 'SP0001', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0019', 'SP0020', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0020', 'SP0038', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0020', 'SP0041', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0021', 'SP0033', 3, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0022', 'SP0025', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0023', 'SP0002', 2, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0024', 'SP0046', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0025', 'SP0035', 2, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0026', 'SP0027', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0027', 'SP0020', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0028', 'SP0019', 5, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0029', 'SP0002', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0030', 'SP0043', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0031', 'SP0003', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0031', 'SP0013', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0031', 'SP0020', 2, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0032', 'SP0022', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0032', 'SP0023', 2, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0032', 'SP0042', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0033', 'SP0012', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0033', 'SP0019', 4, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0033', 'SP0031', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0034', 'SP0035', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0034', 'SP0040', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0034', 'SP0046', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0035', 'SP0004', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0035', 'SP0007', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0035', 'SP0009', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0036', 'SP0002', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0037', 'SP0018', 2, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0038', 'SP0041', 2, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0039', 'SP0037', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0040', 'SP0045', 3, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0041', 'SP0010', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0042', 'SP0016', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0043', 'SP0024', 4, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0044', 'SP0032', 2, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0045', 'SP0047', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0046', 'SP0005', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0046', 'SP0010', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0046', 'SP0013', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0046', 'SP0017', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0047', 'SP0001', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0047', 'SP0019', 2, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0047', 'SP0029', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0047', 'SP0033', 2, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0048', 'SP0015', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0048', 'SP0019', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0048', 'SP0020', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0048', 'SP0040', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0049', 'SP0042', 2, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0049', 'SP0044', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0049', 'SP0047', 4, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0049', 'SP0050', 2, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0050', 'SP0003', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0050', 'SP0007', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0050', 'SP0008', 1, 0);

insert into tblChiTietDonHang (MaDonHang, MaSanPham, SoLuong, DonGia)
values	('DH0050', 'SP0016', 1, 0);

CREATE OR ALTER TRIGGER trg_ChiTietDonHang
ON tblChiTietDonHang
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    IF TRIGGER_NESTLEVEL() > 1 RETURN;

    ---------------------------------------------------------
    -- 1. Tập hợp các dòng đơn hàng bị ảnh hưởng
    ---------------------------------------------------------
    ;WITH Affected AS (
        SELECT MaDonHang, MaSanPham FROM inserted
        UNION
        SELECT MaDonHang, MaSanPham FROM deleted
    )
    SELECT DISTINCT 
        a.MaDonHang, a.MaSanPham, dh.MaChiNhanh
    INTO #A
    FROM Affected a
    JOIN tblDonHang dh ON dh.MaDonHang = a.MaDonHang;

    ---------------------------------------------------------
    -- 2. Tự động lấy đơn giá từ bảng sản phẩm (chỉ khi INSERT)
    ---------------------------------------------------------
    UPDATE ct
    SET ct.DonGia = sp.GiaBan
    FROM tblChiTietDonHang ct
    JOIN inserted i ON i.MaDonHang = ct.MaDonHang AND i.MaSanPham = ct.MaSanPham
    JOIN tblSanPham sp ON sp.MaSanPham = ct.MaSanPham
    WHERE i.DonGia IS NULL OR i.DonGia = 0;

    ---------------------------------------------------------
    -- 3. Kiểm tra tồn kho (SoLuongSanBan)
    ---------------------------------------------------------
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN #A a ON a.MaDonHang = i.MaDonHang AND a.MaSanPham = i.MaSanPham
        JOIN tblTonKho tk ON tk.MaSanPham = i.MaSanPham AND tk.MaChiNhanh = a.MaChiNhanh
        WHERE i.SoLuong > tk.SoLuongSanBan
    )
    BEGIN
        RAISERROR(N'Số lượng vượt quá số lượng sẵn bán!',16,1);
        ROLLBACK;
        RETURN;
    END;

    ---------------------------------------------------------
    -- 4. Cập nhật TongTienHang của đơn hàng
    ---------------------------------------------------------
    UPDATE dh
    SET dh.TongTienHang = ISNULL((
        SELECT SUM(SoLuong * DonGia)
        FROM tblChiTietDonHang
        WHERE MaDonHang = dh.MaDonHang
    ),0)
    FROM tblDonHang dh
    JOIN #A a ON a.MaDonHang = dh.MaDonHang;

    DROP TABLE #A;
END;
GO


CREATE OR ALTER TRIGGER trg_UpdateSoLuongSanBan
ON tblChiTietDonHang
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Lấy danh sách sản phẩm + chi nhánh bị ảnh hưởng
    ;WITH Affected AS (
        SELECT ct.MaSanPham, dh.MaChiNhanh
        FROM inserted i
        JOIN tblDonHang dh ON dh.MaDonHang = i.MaDonHang
        JOIN tblChiTietDonHang ct ON ct.MaDonHang = dh.MaDonHang AND ct.MaSanPham = i.MaSanPham
        UNION
        SELECT ct.MaSanPham, dh.MaChiNhanh
        FROM deleted d
        JOIN tblDonHang dh ON dh.MaDonHang = d.MaDonHang
        JOIN tblChiTietDonHang ct ON ct.MaDonHang = dh.MaDonHang AND ct.MaSanPham = d.MaSanPham
    )
    SELECT DISTINCT * INTO #Affected FROM Affected;

    -- Cập nhật SoLuongSanBan theo đơn hàng 'Chờ xác nhận'
    UPDATE tk
    SET tk.SoLuongSanBan = tk.SoLuongTon - ISNULL(sub.DangCho,0) - 10,
        tk.NgayCapNhat = GETDATE()
    FROM tblTonKho tk
    JOIN #Affected a ON a.MaSanPham = tk.MaSanPham AND a.MaChiNhanh = tk.MaChiNhanh
    OUTER APPLY (
        SELECT SUM(ct.SoLuong) AS DangCho
        FROM tblChiTietDonHang ct
        JOIN tblDonHang dh ON dh.MaDonHang = ct.MaDonHang
        WHERE dh.TrangThaiDonHang = N'Chờ xác nhận'
          AND dh.MaChiNhanh = tk.MaChiNhanh
          AND ct.MaSanPham = tk.MaSanPham
    ) sub;

    DROP TABLE #Affected;
END
GO

-------------------------------------------------
-- TRIGGER CT ĐƠN HÀNG (MỚI)
-------------------------------------------------
CREATE OR ALTER TRIGGER trg_ChiTietDonHang_Full
ON tblChiTietDonHang
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    IF TRIGGER_NESTLEVEL() > 1 RETURN;

    -------------------------------------------------
    -- 1. Tập hợp các dòng bị ảnh hưởng
    -------------------------------------------------
    ;WITH Affected AS (
        SELECT MaDonHang, MaSanPham FROM inserted
        UNION
        SELECT MaDonHang, MaSanPham FROM deleted
    )
    SELECT DISTINCT a.MaDonHang, a.MaSanPham, dh.MaChiNhanh
    INTO #Affected
    FROM Affected a
    JOIN tblDonHang dh ON dh.MaDonHang = a.MaDonHang;

    -------------------------------------------------
    -- 2. Cập nhật DonGia cho INSERT hoặc DonGia = 0
    -------------------------------------------------
    UPDATE ct
    SET ct.DonGia = sp.GiaBan
    FROM tblChiTietDonHang ct
    JOIN inserted i ON i.MaDonHang = ct.MaDonHang AND i.MaSanPham = ct.MaSanPham
    JOIN tblSanPham sp ON sp.MaSanPham = ct.MaSanPham
    WHERE i.DonGia IS NULL OR i.DonGia = 0;

    -------------------------------------------------
    -- 3. Kiểm tra tồn kho cho INSERT/UPDATE
    -------------------------------------------------
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN #Affected a ON a.MaDonHang = i.MaDonHang AND a.MaSanPham = i.MaSanPham
        JOIN tblTonKho tk ON tk.MaSanPham = i.MaSanPham AND tk.MaChiNhanh = a.MaChiNhanh
        WHERE i.SoLuong > tk.SoLuongSanBan
    )
    BEGIN
        RAISERROR(N'Số lượng vượt quá số lượng sẵn bán!',16,1);
        ROLLBACK;
        RETURN;
    END;

   
    -------------------------------------------------
    -- 5. Cập nhật TongTienHang trong tblDonHang
    -------------------------------------------------
    UPDATE dh
    SET dh.TongTienHang = ISNULL(ct.Total,0)
    FROM tblDonHang dh
    JOIN (
        SELECT MaDonHang, SUM(SoLuong*DonGia) AS Total
        FROM tblChiTietDonHang
        GROUP BY MaDonHang
    ) ct ON ct.MaDonHang = dh.MaDonHang
    WHERE dh.MaDonHang IN (SELECT MaDonHang FROM #Affected);

    DROP TABLE #Affected;
END;
GO



---------------------------------------------------------------------
-- Tự động cập nhật điểm tích lũy
---------------------------------------------------------------------
CREATE OR ALTER TRIGGER trg_CapNhatDiemTichLuy
ON tblChiTietDiemTichLuy
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    IF TRIGGER_NESTLEVEL() > 1 RETURN;

    ---------------------------------------------------------------------
    -- 0. Nếu là DELETE thì bỏ qua (cho phép xóa khi xóa DonHang)
    ---------------------------------------------------------------------
    IF NOT EXISTS(SELECT 1 FROM inserted)
        RETURN;

    ---------------------------------------------------------------------
    -- 1. Ngăn INSERT thủ công không có đơn hàng
    ---------------------------------------------------------------------
    IF EXISTS (
        SELECT 1
        FROM inserted i
        LEFT JOIN tblDonHang dh ON dh.MaDonHang = i.MaDonHang
        WHERE dh.MaDonHang IS NULL
    )
    BEGIN
        RAISERROR(N'Không được tự thêm điểm tích lũy!',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    ---------------------------------------------------------------------
    -- 2. Bảng tạm tập hợp thay đổi bằng UNION ALL
    ---------------------------------------------------------------------
    DECLARE @BangThayDoi TABLE (
        MaDonHang nchar(6),
        MaKhachHang nchar(6),
        DiemDaSuDungMoi int,
        DiemDaSuDungCu int
    );

    INSERT INTO @BangThayDoi (MaDonHang, MaKhachHang, DiemDaSuDungMoi, DiemDaSuDungCu)
    SELECT i.MaDonHang, i.MaKhachHang, ISNULL(i.DiemDaSuDung,0), 0
    FROM inserted i
    UNION ALL
    SELECT d.MaDonHang, d.MaKhachHang, 0, ISNULL(d.DiemDaSuDung,0)
    FROM deleted d;

    ---------------------------------------------------------------------
    -- 3. Cập nhật tblDonHang.DiemSuDung
    ---------------------------------------------------------------------
    UPDATE dh
    SET DiemSuDung = b.DiemDaSuDungMoi
    FROM tblDonHang dh
    INNER JOIN @BangThayDoi b
        ON dh.MaDonHang = b.MaDonHang
       AND dh.MaKhachHang = b.MaKhachHang;

    ---------------------------------------------------------------------
    -- 4. Cập nhật tblKhachHang (Thành viên)
    ---------------------------------------------------------------------
    UPDATE kh
    SET DiemTichLuy = kh.DiemTichLuy - b.DiemDaSuDungCu + b.DiemDaSuDungMoi
    FROM tblKhachHang kh
    INNER JOIN @BangThayDoi b
        ON kh.MaKhachHang = b.MaKhachHang
    WHERE kh.LoaiKhachHang = N'Thành viên';

    ---------------------------------------------------------------------
    -- 5. Cập nhật hạn sử dụng điểm
    ---------------------------------------------------------------------
    UPDATE ctd
    SET NgayHetHan = DATEFROMPARTS(YEAR(GETDATE())+2,12,31)
    FROM tblChiTietDiemTichLuy ctd
    INNER JOIN @BangThayDoi b
        ON ctd.MaDonHang = b.MaDonHang
       AND ctd.MaKhachHang = b.MaKhachHang;
END;
GO

									--------------------------------------------------------------------
															-- BẢNG THANH TOÁN --
                                    --------------------------------------------------------------------

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0001', 'KH0010', 'DH0001', 'NV0049', N'Tiền mặt khi giao hàng', '2025-10-27', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0002', 'KH0012', 'DH0002', 'NV0049', N'Thẻ ATM', '2025-10-27', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0003', 'KH0014', 'DH0003', 'NV0049', N'Ví điện tử', '2025-10-27', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0004', 'KH0001', 'DH0004', 'NV0005', N'Thẻ tín dụng', '2025-10-25', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0005', 'KH0002', 'DH0005', 'NV0005', N'Chuyển khoản', '2025-10-26', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0006', 'KH0005', 'DH0006', 'NV0005', N'Thẻ ghi nợ', '2025-10-26', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0007', 'KH0006', 'DH0007', 'NV0005', N'Ví điện tử', '2025-10-26', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0008', 'KH0007', 'DH0008', 'NV0005', N'Thẻ tín dụng', '2025-10-26', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0009', 'KH0008', 'DH0009', 'NV0005', N'Thanh toán QR', '2025-10-26', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0010', 'KH0043', 'DH0010', 'NV0014', N'Chuyển khoản', '2025-10-26', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0011', 'KH0050', 'DH0011', 'NV0014', N'Thẻ ATM', '2025-10-26', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0012', 'KH0051', 'DH0012', 'NV0014', N'Thẻ tín dụng', '2025-10-27', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0013', 'KH0052', 'DH0013', 'NV0014', N'Chuyển khoản', '2025-10-27', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0014', 'KH0053', 'DH0014', 'NV0014', N'Thẻ ghi nợ', '2025-10-28', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0015', 'KH0055', 'DH0015', 'NV0014', N'Ví điện tử', '2025-10-28', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0016', 'KH0021', 'DH0016', 'NV0022', N'Thanh toán QR', '2025-10-28', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0017', 'KH0022', 'DH0017', 'NV0022', N'Thẻ tín dụng', '2025-10-28', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0018', 'KH0025', 'DH0018', 'NV0022', N'Chuyển khoản', '2025-10-29', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0019', 'KH0042', 'DH0019', 'NV0022', N'Thẻ ATM', '2025-10-29', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0020', 'KH0049', 'DH0020', 'NV0030', N'Thẻ tín dụng', '2025-10-29', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0021', 'KH0056', 'DH0021', 'NV0030', N'Chuyển khoản', '2025-10-29', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0022', 'KH0058', 'DH0022', 'NV0030', N'Thẻ ghi nợ', '2025-10-29', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0023', 'KH0015', 'DH0023', 'NV0038', N'Ví điện tử', '2025-10-29', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0024', 'KH0016', 'DH0024', 'NV0038', N'Thanh toán QR', '2025-10-29', 0, 1);

insert into tblThanhToan (MaThanhToan, MaKhachHang, MaDonHang, MaNhanVien, PhuongThucThanhToan, NgayThanhToan, TongTien, TrangThaiThanhToan)
values ('TT0025', 'KH0019', 'DH0025', 'NV0038', N'Thẻ ATM', '2025-10-30', 0, 1);


--Tự động xử lý thanh toán
---------------------------------------------------------------------
-- Tự động xử lý thanh toán (chặn khi người dùng sửa Tổng tiền thủ công)
---------------------------------------------------------------------
CREATE OR ALTER TRIGGER trg_XuLyThanhToan
ON tblThanhToan
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- CHẶN sửa MaDonHang
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN deleted d ON i.MaThanhToan = d.MaThanhToan
        WHERE i.MaDonHang <> d.MaDonHang
    )
    BEGIN
        RAISERROR (N'Không được phép thay đổi Đơn hàng của Thanh toán.',16,1);
        ROLLBACK;
        RETURN;
    END

    -- INSERT: TỰ SET TongTien = DonHang.ThanhTien
    UPDATE tt
    SET TongTien = dh.ThanhTien
    FROM tblThanhToan tt
    JOIN inserted i ON tt.MaThanhToan = i.MaThanhToan
    JOIN tblDonHang dh ON dh.MaDonHang = i.MaDonHang
    LEFT JOIN deleted d ON d.MaThanhToan = i.MaThanhToan
    WHERE d.MaThanhToan IS NULL;

    -- UPDATE: CHẶN sửa TongTien
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN deleted d ON i.MaThanhToan = d.MaThanhToan
        WHERE ISNULL(i.TongTien,0) <> ISNULL(d.TongTien,0)
    )
    BEGIN
        RAISERROR (N'Không được phép chỉnh sửa Tổng tiền thanh toán.',16,1);
        ROLLBACK;
        RETURN;
    END
END


									--------------------------------------------------------------------
															-- BẢNG HÓA ĐƠN --
                                    --------------------------------------------------------------------

-- Dữ liệu Hóa đơn
insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0001', 'TT0001', 'NV0049', '2025-10-27', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0002', 'TT0002', 'NV0049', '2025-10-27', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0003', 'TT0003', 'NV0049', '2025-10-27', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0004', 'TT0004', 'NV0005', '2025-10-25', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0005', 'TT0005', 'NV0005', '2025-10-26', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0006', 'TT0006', 'NV0005', '2025-10-26', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0007', 'TT0007', 'NV0005', '2025-10-26', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0008', 'TT0008', 'NV0005', '2025-10-26', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0009', 'TT0009', 'NV0005', '2025-10-26', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0010', 'TT0010', 'NV0014', '2025-10-26', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0011', 'TT0011', 'NV0014', '2025-10-26', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0012', 'TT0012', 'NV0014', '2025-10-27', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0013', 'TT0013', 'NV0014', '2025-10-27', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0014', 'TT0014', 'NV0014', '2025-10-28', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0015', 'TT0015', 'NV0014', '2025-10-28', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0016', 'TT0016', 'NV0022', '2025-10-28', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0017', 'TT0017', 'NV0022', '2025-10-28', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0018', 'TT0018', 'NV0022', '2025-10-29', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0019', 'TT0019', 'NV0022', '2025-10-29', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0020', 'TT0020', 'NV0030', '2025-10-29', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0021', 'TT0021', 'NV0030', '2025-10-29', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0022', 'TT0022', 'NV0030', '2025-10-29', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0023', 'TT0023', 'NV0038', '2025-10-29', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0024', 'TT0024', 'NV0038', '2025-10-29', 0, 0, 0);

insert into tblHoaDon (MaHoaDon, MaThanhToan, MaNhanVien, NgayXuatHoaDon, TongTien, SoTienDaThanhToan, SoDuConLai)
values	('HD0025', 'TT0025', 'NV0038', '2025-10-30', 0, 0, 0);


---------------------------------------------------------------------
-- Cập nhật xử lý hóa đơn
---------------------------------------------------------------------
CREATE OR ALTER TRIGGER trg_XuLyHoaDon
ON tblHoaDon
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Chống đệ quy trigger
    IF TRIGGER_NESTLEVEL() > 1 RETURN;

    ---------------------------------------------------------------------
    -- 1. Lấy dữ liệu cần thiết từ inserted + thanh toán + đơn hàng
    ---------------------------------------------------------------------
    ;WITH CTE AS (
        SELECT
            hd.MaHoaDon,
            hd.MaThanhToan,
            hd.NgayXuatHoaDon AS NgayXuatHD_Nhap,
            hd.TongTien        AS TongTien_Nhap,
            hd.SoTienDaThanhToan AS SoTienDaTT_Nhap,
            hd.SoDuConLai      AS SoDuConLai_Nhap,

            tt.TrangThaiThanhToan,
            tt.NgayThanhToan,
            tt.TongTien AS TongTienThanhToan,

            dh.ThanhTien AS TongTienDonHang
        FROM inserted hd
        JOIN tblThanhToan tt ON hd.MaThanhToan = tt.MaThanhToan
        JOIN tblDonHang dh ON tt.MaDonHang = dh.MaDonHang
    )
    SELECT * INTO #Check FROM CTE;

    ---------------------------------------------------------------------
    -- 3. Kiểm tra logic khi UPDATE (người dùng sửa sai)
    ---------------------------------------------------------------------
    IF EXISTS (SELECT 1 FROM deleted)
    BEGIN
        -- Kiểm tra Ngày xuất hóa đơn phải khớp
        IF EXISTS (
            SELECT 1 FROM #Check
            WHERE DATEDIFF(day, NgayXuatHD_Nhap, NgayThanhToan) <> 0
        )
        BEGIN
            RAISERROR(N'Ngày xuất hóa đơn phải bằng Ngày thanh toán!', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        -- Kiểm tra Tổng tiền phải khớp
        IF EXISTS (
            SELECT 1 FROM #Check
            WHERE TongTien_Nhap <> TongTienDonHang
        )
        BEGIN
            RAISERROR(N'Tổng tiền hóa đơn phải bằng Thành tiền của đơn hàng!', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        -- Kiểm tra số tiền đã thanh toán phải khớp
        IF EXISTS (
            SELECT 1 FROM #Check
            WHERE SoTienDaTT_Nhap <> 
                CASE WHEN TrangThaiThanhToan = 1 
                     THEN TongTienThanhToan 
                     ELSE 0 END
        )
        BEGIN
            RAISERROR(N'Số tiền đã thanh toán phải bằng Tổng tiền trong thanh toán!', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        -- Kiểm tra Số dư còn lại không được sửa
        IF EXISTS (
            SELECT 1 FROM #Check
            WHERE SoDuConLai_Nhap <> 
                (TongTienDonHang - 
                    CASE WHEN TrangThaiThanhToan = 1 
                         THEN TongTienThanhToan 
                         ELSE 0 END)
        )
        BEGIN
            RAISERROR(N'Số dư còn lại không hợp lệ!', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END;
    END;


    ---------------------------------------------------------------------
    -- 4. Cập nhật lại dữ liệu chính xác cho INSERT hoặc UPDATE
    ---------------------------------------------------------------------
    UPDATE hd
    SET 
        hd.NgayXuatHoaDon    = ISNULL(c.NgayThanhToan, GETDATE()),

        -- Tổng tiền hóa đơn = tổng tiền đơn hàng
        hd.TongTien          = c.TongTienDonHang,

        -- Số tiền đã thanh toán theo trạng thái
        hd.SoTienDaThanhToan =
            CASE WHEN c.TrangThaiThanhToan = 1 
                 THEN c.TongTienThanhToan 
                 ELSE 0 END,

        -- Số dư
        hd.SoDuConLai        = c.TongTienDonHang -
            CASE WHEN c.TrangThaiThanhToan = 1 
                 THEN c.TongTienThanhToan 
                 ELSE 0 END
    FROM tblHoaDon hd
    JOIN #Check c ON hd.MaHoaDon = c.MaHoaDon;

    DROP TABLE #Check;
END
GO


									--------------------------------------------------------------------
															-- BẢNG PHIẾU NHẬP --
                                    --------------------------------------------------------------------
-- Dữ liệu phiếu nhập
insert into tblPhieuNhap (MaPhieuNhap, MaNhanVien, MaNhaCungCap, MaChiNhanh, TongTien, NgayNhap)
values	
		('PN0001', 'NV0007', 'NCC001', 'CN01', 0, '2025-10-21'),
		('PN0002', 'NV0016', 'NCC003', 'CN02', 0, '2025-10-21'),
		('PN0003', 'NV0024', 'NCC009', 'CN03', 0, '2025-10-22'),
		('PN0004', 'NV0024', 'NCC005', 'CN03', 0, '2025-10-22'),
		('PN0005', 'NV0032', 'NCC006', 'CN04', 0, '2025-10-22'),
		('PN0006', 'NV0040', 'NCC002', 'CN05', 0, '2025-10-22'),
		('PN0007', 'NV0040', 'NCC010', 'CN05', 0, '2025-10-22'),
		('PN0008', 'NV0047', 'NCC001', 'CN06', 0, '2025-10-23'),
		('PN0009', 'NV0049', 'NCC008', 'CN06', 0, '2025-10-23');

--Tự động tính tổng tiền của mỗi phiếu nhập
CREATE TRIGGER trg_KiemTraTongTienPhieuNhap
ON tblPhieuNhap
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Nếu TongTien không bị sửa → không làm gì
    IF NOT EXISTS (
        SELECT 1 
        FROM inserted i
        JOIN deleted d ON i.MaPhieuNhap = d.MaPhieuNhap
        WHERE i.TongTien <> d.TongTien
    )
        RETURN;

    -- 2. Kiểm tra tổng tiền đúng từ chi tiết
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN (
            SELECT MaPhieuNhap, SUM(SoLuong * GiaNhap) AS TongDung
            FROM tblCTPhieuNhap
            WHERE MaPhieuNhap IN (SELECT MaPhieuNhap FROM inserted)
            GROUP BY MaPhieuNhap
        ) t ON i.MaPhieuNhap = t.MaPhieuNhap
        WHERE i.TongTien <> t.TongDung
    )
    BEGIN
        RAISERROR(N'Tổng tiền không chính xác! Vui lòng kiểm tra lại.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

									--------------------------------------------------------------------
														-- BẢNG CHI TIẾT PHIẾU NHẬP --
                                    --------------------------------------------------------------------
insert into tblCTPhieuNhap (MaPhieuNhap, MaSanPham, SoLuong, GiaNhap, NgaySanXuat, HanSuDung)
values	
		('PN0001', 'SP0001', 60, 255999, '2025-09-05', '2028-09-05'),
		('PN0001', 'SP0004', 70, 245000, '2025-07-01', '2028-07-01'),
		('PN0001', 'SP0005', 200, 95999, '2025-08-10', '2028-08-10'),
		('PN0001', 'SP0008', 50, 314000, '2025-08-02', '2028-08-02'),
		('PN0001', 'SP0009', 110, 170000, '2025-07-19', '2028-07-19'),
		('PN0001', 'SP0010', 90, 199999, '2025-06-12', '2028-06-12'),
		('PN0001', 'SP0012', 70, 251000, '2025-08-28', '2028-08-28'),
		('PN0001', 'SP0016', 65, 285000, '2025-07-18', '2028-07-18'),
		('PN0002', 'SP0020', 350, 50857, '2025-07-23', '2028-07-23'),
		('PN0002', 'SP0027', 130, 140571, '2025-09-03', '2028-09-03'),
		('PN0002', 'SP0028', 180, 86000, '2025-08-21', '2028-08-21'),
		('PN0002', 'SP0029', 180, 85714, '2025-07-17', '2028-07-17'),
		('PN0002', 'SP0030', 160, 106857, '2025-09-10', '2028-09-10'),
		('PN0002', 'SP0032', 300, 44571, '2025-07-09', '2028-07-09'),
		('PN0002', 'SP0035', 150, 117714, '2025-07-15', '2028-07-15'),
		('PN0002', 'SP0037', 220, 60000, '2025-08-13', '2028-08-13'),
		('PN0003', 'SP0013', 100, 176000, '2025-07-16', '2028-07-16'),
		('PN0003', 'SP0018', 210, 81714, '2025-07-06', '2027-07-06'),
		('PN0003', 'SP0026', 210, 70000, '2025-07-20', '2027-07-20'),
		('PN0004', 'SP0031', 160, 107999, '2025-08-24', '2028-08-24'),
		('PN0004', 'SP0045', 280, 46000, '2025-09-20', '2028-09-20'),
		('PN0004', 'SP0046', 200, 73714, '2025-08-31', '2028-08-31'),
		('PN0005', 'SP0006', 40, 505000, '2025-07-25', '2028-07-25'),
		('PN0005', 'SP0007', 180, 102857, '2025-09-08', '2028-09-08'),
		('PN0005', 'SP0014', 60, 237000, '2025-09-04', '2028-09-04'),
		('PN0005', 'SP0017', 110, 168571, '2025-09-11', '2028-09-11'),
		('PN0006', 'SP0011', 45, 401000, '2025-09-07', '2028-09-07'),
		('PN0007', 'SP0042', 110, 154000, '2025-08-25', '2028-08-25'),
		('PN0007', 'SP0043', 50, 376000, '2025-07-12', '2028-07-12'),
		('PN0007', 'SP0044', 160, 101000, '2025-09-06', '2028-09-06'),
		('PN0008', 'SP0001', 100, 255999, '2025-09-05', '2028-09-05'),
		('PN0008', 'SP0004', 80, 245000, '2025-07-01', '2028-07-01'),
		('PN0008', 'SP0036', 150, 113714, '2025-08-08', '2028-08-08'),
		('PN0008', 'SP0039', 100, 147999, '2025-08-04', '2028-08-04'),
		('PN0008', 'SP0040', 90, 165000, '2025-09-18', '2028-09-18'),
		('PN0008', 'SP0041', 120, 130857, '2025-09-02', '2028-09-02'),
		('PN0009', 'SP0002', 120, 142000, '2025-08-20', '2028-08-20'),
		('PN0009', 'SP0003', 130, 159999, '2025-09-15', '2028-09-15'),
		('PN0009', 'SP0023', 200, 71000, '2025-07-05', '2028-07-05'),
		('PN0009', 'SP0025', 40, 486857, '2025-09-14', '2028-09-14');

ENABLE TRIGGER trg_CapNhapCTPhieuNhap ON tblCTPhieuNhap;
---------------------------------------------------------------------
-- SEQUENCE + DEFAULT cho Mã tồn kho (CHẠY 1 LẦN)
---------------------------------------------------------------------
CREATE SEQUENCE seq_MaTonKho
    START WITH 1
    INCREMENT BY 1;
GO

ALTER TABLE tblTonKho
ADD CONSTRAINT DF_MaTonKho
DEFAULT ('TK' + RIGHT('0000' + CAST(NEXT VALUE FOR seq_MaTonKho AS VARCHAR(4)),4))
FOR MaTonKho;
GO

---------------------------------------------------------------------
-- Trigger Cập nhật chi tiết phiếu nhập
---------------------------------------------------------------------
CREATE OR ALTER TRIGGER trg_CapNhapCTPhieuNhap
ON tblCTPhieuNhap
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CTPN TABLE (
        MaPhieuNhap nchar(6),
        MaSanPham   nchar(6),
        SL_Moi      int,
        SL_Cu       int
    );

    INSERT INTO @CTPN
    SELECT 
        COALESCE(i.MaPhieuNhap, d.MaPhieuNhap),
        COALESCE(i.MaSanPham, d.MaSanPham),
        COALESCE(i.SoLuong, 0),
        COALESCE(d.SoLuong, 0)
    FROM inserted i
    FULL JOIN deleted d
        ON i.MaPhieuNhap = d.MaPhieuNhap
       AND i.MaSanPham = d.MaSanPham;

    -- CẬP NHẬT TỒN KHO
    UPDATE tk
    SET tk.SoLuongTon = tk.SoLuongTon + (t.SL_Moi - t.SL_Cu),
        tk.NgayCapNhat = GETDATE()
    FROM tblTonKho tk
    JOIN @CTPN t ON tk.MaSanPham = t.MaSanPham
    JOIN tblPhieuNhap pn ON pn.MaPhieuNhap = t.MaPhieuNhap
    WHERE tk.MaChiNhanh = pn.MaChiNhanh;
END;
GO



									--------------------------------------------------------------------
															-- BẢNG PHIẾU XUẤT --
                                    --------------------------------------------------------------------

-- Dữ liệu phiếu xuất
insert into tblPhieuXuat (MaPhieuXuat, MaNhanVien, MaDonHang, MaChiNhanh, NgayXuat, TongTien)
values	('PX0001', 'NV0049', 'DH0022', 'CN05', '2025-10-29', 0),
		('PX0002', 'NV0049', 'DH0023', 'CN04', '2025-10-29', 0),
		('PX0003', 'NV0049', 'DH0024', 'CN04', '2025-10-29', 0),
		('PX0004', 'NV0049', 'DH0025', 'CN04', '2025-10-30', 0),
		('PX0005', 'NV0049', 'DH0036', 'CN06', '2025-11-02', 0),
		('PX0006', 'NV0049', 'DH0037', 'CN06', '2025-11-03', 0);


									--------------------------------------------------------------------
														-- BẢNG CHI TIẾT PHIẾU XUẤT --
                                    --------------------------------------------------------------------

--Dữ liệu chi tiết phiếu xuất
insert into tblCTPhieuXuat (MaPhieuXuat, MaSanPham, SoLuong, DonGia)
values	('PX0001', 'SP0025', 1, 0),
		('PX0002', 'SP0002', 2, 0),
		('PX0003', 'SP0046', 1, 0),
		('PX0004', 'SP0035', 2, 0),
		('PX0005', 'SP0002', 1, 0),
		('PX0006', 'SP0018', 2, 0);

--
CREATE OR ALTER TRIGGER trg_CTPhieuXuat
ON tblCTPhieuXuat
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Tránh đệ quy
    IF TRIGGER_NESTLEVEL() > 1 RETURN;

    ---------------------------------------------------------
    -- Bảng tạm lưu dữ liệu thay đổi
    ---------------------------------------------------------
    DECLARE @Temp TABLE (
        MaPhieuXuat nchar(6),
        MaSanPham nchar(6),
        SoLuong int,
        DonGia decimal(18,2)
    );

    INSERT INTO @Temp
    SELECT MaPhieuXuat, MaSanPham, SoLuong, DonGia FROM inserted;

    ---------------------------------------------------------
    -- Lấy dữ liệu cần thiết từ phiếu xuất và đơn hàng
    ---------------------------------------------------------
    ;WITH CTE AS (
        SELECT t.MaPhieuXuat, t.MaSanPham, t.SoLuong, t.DonGia,
               px.MaDonHang, px.MaChiNhanh,
               ctdh.SoLuong AS SL_DH, ctdh.DonGia AS Gia_DH,
               dh.TrangThaiDonHang
        FROM @Temp t
        JOIN tblPhieuXuat px ON px.MaPhieuXuat = t.MaPhieuXuat
        JOIN tblDonHang dh ON dh.MaDonHang = px.MaDonHang
        JOIN tblChiTietDonHang ctdh ON ctdh.MaDonHang = dh.MaDonHang
                                   AND ctdh.MaSanPham = t.MaSanPham
    )
    SELECT * INTO #CTE FROM CTE;

    ---------------------------------------------------------
    -- 1. Kiểm tra trạng thái đơn hàng
    ---------------------------------------------------------
    IF EXISTS (
    SELECT 1
    FROM #CTE c
    JOIN tblDonHang dh ON dh.MaDonHang = c.MaDonHang
    WHERE (dh.LoaiDonHang = N'Trực tuyến' AND dh.TrangThaiDonHang <> N'Chờ lấy hàng')
       OR (dh.LoaiDonHang = N'Tại cửa hàng' AND dh.TrangThaiDonHang <> N'Giao hàng thành công')
	)
	BEGIN
		RAISERROR(N'Không thể tạo phiếu xuất: kiểm tra loại đơn hàng và trạng thái đơn hàng!',16,1);
		ROLLBACK TRANSACTION;
		RETURN;
	END

    ---------------------------------------------------------
    -- 2. Cập nhật số lượng và đơn giá nếu đang để 0
    ---------------------------------------------------------
    UPDATE px
    SET SoLuong = c.SL_DH,
        DonGia  = c.Gia_DH
    FROM tblCTPhieuXuat px
    JOIN #CTE c ON px.MaPhieuXuat = c.MaPhieuXuat
                AND px.MaSanPham = c.MaSanPham
    WHERE px.SoLuong = 0 OR px.DonGia = 0;

    ---------------------------------------------------------
    -- 3. Kiểm tra update thủ công (sai số lượng/giá) → rollback
    ---------------------------------------------------------
    IF EXISTS (
        SELECT 1
        FROM tblCTPhieuXuat px
        JOIN #CTE c ON px.MaPhieuXuat = c.MaPhieuXuat
                   AND px.MaSanPham = c.MaSanPham
        WHERE (px.SoLuong <> c.SL_DH OR px.DonGia <> c.Gia_DH)
              AND NOT (px.SoLuong = 0 OR px.DonGia = 0)
    )
    BEGIN
        RAISERROR(N'Số lượng hoặc đơn giá không khớp với chi tiết đơn hàng.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    ---------------------------------------------------------
    -- 4. Kiểm tra tồn kho
    ---------------------------------------------------------
    ;WITH TonKhoCTE AS (
        SELECT tk.MaSanPham, tk.MaChiNhanh, tk.SoLuongTon,
               c.SoLuong AS XuatSL
        FROM #CTE c
        JOIN tblTonKho tk ON tk.MaSanPham = c.MaSanPham
                        AND tk.MaChiNhanh = c.MaChiNhanh
    )
    SELECT * INTO #TonKhoCheck FROM TonKhoCTE;

    DECLARE @BadStock int;
    -- Tồn kho <=10 → rollback
    SELECT @BadStock = COUNT(*) 
    FROM #TonKhoCheck
    WHERE SoLuongTon - XuatSL <= 10 AND SoLuongTon - XuatSL > 0;

    IF @BadStock > 0
    BEGIN
        RAISERROR(N'Cảnh báo: Tồn kho sau xuất <= 10, không được phép xuất.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- Tồn kho = 0 → cảnh báo
    IF EXISTS (SELECT 1 FROM #TonKhoCheck WHERE SoLuongTon - XuatSL = 0)
        PRINT N'Cảnh báo: Tồn kho sau xuất bằng 0.';

    ---------------------------------------------------------
    -- 5. CẬP NHẬT TỒN KHO (INSERT / UPDATE / DELETE)
    ---------------------------------------------------------
    UPDATE tk
    SET tk.SoLuongTon =
        tk.SoLuongTon
        - ISNULL(i.SoLuong,0)
        + ISNULL(d.SoLuong,0),
        tk.NgayCapNhat = GETDATE()
    FROM tblTonKho tk
    JOIN tblPhieuXuat px ON px.MaChiNhanh = tk.MaChiNhanh
    LEFT JOIN inserted i
        ON i.MaSanPham = tk.MaSanPham
       AND i.MaPhieuXuat = px.MaPhieuXuat
    LEFT JOIN deleted d
        ON d.MaSanPham = tk.MaSanPham
       AND d.MaPhieuXuat = px.MaPhieuXuat;

END
GO


CREATE TRIGGER trg_TongTienPhieuXuat
ON tblCTPhieuXuat
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Tránh đệ quy trigger
    IF TRIGGER_NESTLEVEL() > 1 RETURN;

    ---------------------------------------------------------
    -- Lấy tất cả phiếu xuất vừa thay đổi
    ---------------------------------------------------------
    ;WITH PX_Changed AS (
        SELECT MaPhieuXuat FROM inserted
        UNION
        SELECT MaPhieuXuat FROM deleted
    )
    ---------------------------------------------------------
    -- Cập nhật TongTien trong tblPhieuXuat dựa trên chi tiết
    ---------------------------------------------------------
    UPDATE p
    SET TongTien = ISNULL(ct.TongTienCT, 0)
    FROM tblPhieuXuat p
    JOIN PX_Changed pc ON p.MaPhieuXuat = pc.MaPhieuXuat
    LEFT JOIN (
        SELECT MaPhieuXuat, SUM(SoLuong * DonGia) AS TongTienCT
        FROM tblCTPhieuXuat
        GROUP BY MaPhieuXuat
    ) ct ON p.MaPhieuXuat = ct.MaPhieuXuat;
END;
GO

									--------------------------------------------------------------------
															-- BẢNG TỒN KHO --
                                    --------------------------------------------------------------------
CREATE OR ALTER TRIGGER trg_CapNhapTonKho
ON tblTonKho
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Tránh đệ quy trigger
    --IF TRIGGER_NESTLEVEL() > 1 RETURN;

    ---------------------------------------------------------
    -- 1. Không cho phép sửa trực tiếp SoLuongSanBan
    ---------------------------------------------------------
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN deleted d ON i.MaTonKho = d.MaTonKho
        WHERE ISNULL(i.SoLuongSanBan,0) <> ISNULL(d.SoLuongSanBan,0)
    )
    BEGIN
        RAISERROR(N'Không được phép sửa trực tiếp số lượng sẵn bán!',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    ---------------------------------------------------------
    -- 2. Cập nhật SoLuongSanBan = SoLuongTon - 10
    -- (chạy cho cả INSERT & UPDATE)
    ---------------------------------------------------------
    UPDATE tk
    SET tk.SoLuongSanBan = tk.SoLuongTon - 10,
        tk.NgayCapNhat = GETDATE()
    FROM tblTonKho tk
    JOIN inserted i 
        ON tk.MaTonKho = i.MaTonKho;

    ---------------------------------------------------------
    -- 3. Kiểm tra giới hạn SoLuongTon
    ---------------------------------------------------------
    IF EXISTS (
        SELECT 1
        FROM tblTonKho tk
        JOIN inserted i ON tk.MaTonKho = i.MaTonKho
        WHERE tk.SoLuongTon <= 10 OR tk.SoLuongTon >= 1000
    )
    BEGIN
        RAISERROR(N'Số lượng tồn kho phải lớn hơn 10 và nhỏ hơn 1000!',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END
GO


insert into tblTonKho (MaTonKho, MaSanPham, MaChiNhanh, MaNhanVien, SoLuongSanBan, SoLuongTon, NgayCapNhat)
values	
-- CN01 (MaNhanVien: NV0007)
		('TK0002', 'SP0002', 'CN06', 'NV0007', 0, 100, '2025-10-20');

-- Dữ liệu tồn kho
insert into tblTonKho (MaTonKho, MaSanPham, MaChiNhanh, MaNhanVien, SoLuongSanBan, SoLuongTon, NgayCapNhat)
values	
-- CN01 (MaNhanVien: NV0007)
		('TK0001', 'SP0001', 'CN01', 'NV0007', 0, 58, '2025-10-20'),
		('TK0002', 'SP0002', 'CN01', 'NV0007', 0, 118, '2025-10-20'),
		('TK0003', 'SP0003', 'CN01', 'NV0007', 0, 135, '2025-10-20'),
		('TK0004', 'SP0004', 'CN01', 'NV0007', 0, 68, '2025-10-20'),
		('TK0005', 'SP0005', 'CN01', 'NV0007', 0, 205, '2025-10-20'),
		('TK0006', 'SP0006', 'CN01', 'NV0007', 0, 42, '2025-10-20'),
		('TK0007', 'SP0007', 'CN01', 'NV0007', 0, 185, '2025-10-20'),
		('TK0008', 'SP0008', 'CN01', 'NV0007', 0, 53, '2025-10-20'),
		('TK0009', 'SP0009', 'CN01', 'NV0007', 0, 115, '2025-10-20'),
		('TK0010', 'SP0010', 'CN01', 'NV0007', 0, 92, '2025-10-20'),
		('TK0011', 'SP0011', 'CN01', 'NV0007', 0, 47, '2025-10-20'),
		('TK0012', 'SP0012', 'CN01', 'NV0007', 0, 75, '2025-10-20'),
		('TK0013', 'SP0013', 'CN01', 'NV0007', 0, 98, '2025-10-20'),
		('TK0014', 'SP0014', 'CN01', 'NV0007', 0, 63, '2025-10-20'),
		('TK0015', 'SP0015', 'CN01', 'NV0007', 0, 28, '2025-10-20'),
		('TK0016', 'SP0016', 'CN01', 'NV0007', 0, 67, '2025-10-20'),
		('TK0017', 'SP0017', 'CN01', 'NV0007', 0, 108, '2025-10-20'),
		('TK0018', 'SP0018', 'CN01', 'NV0007', 0, 215, '2025-10-20'),
		('TK0019', 'SP0019', 'CN01', 'NV0007', 0, 425, '2025-10-20'),
		('TK0020', 'SP0020', 'CN01', 'NV0007', 0, 345, '2025-10-20'),
		('TK0021', 'SP0021', 'CN01', 'NV0007', 0, 103, '2025-10-20'),
		('TK0022', 'SP0022', 'CN01', 'NV0007', 0, 88, '2025-10-20'),
		('TK0023', 'SP0023', 'CN01', 'NV0007', 0, 205, '2025-10-20'),
		('TK0024', 'SP0024', 'CN01', 'NV0007', 0, 188, '2025-10-20'),
		('TK0025', 'SP0025', 'CN01', 'NV0007', 0, 42, '2025-10-20'),
		('TK0026', 'SP0026', 'CN01', 'NV0007', 0, 208, '2025-10-20'),
		('TK0027', 'SP0027', 'CN01', 'NV0007', 0, 128, '2025-10-20'),
		('TK0028', 'SP0028', 'CN01', 'NV0007', 0, 182, '2025-10-20'),
		('TK0029', 'SP0029', 'CN01', 'NV0007', 0, 178, '2025-10-20'),
		('TK0030', 'SP0030', 'CN01', 'NV0007', 0, 165, '2025-10-20'),
		('TK0031', 'SP0031', 'CN01', 'NV0007', 0, 158, '2025-10-20'),
		('TK0032', 'SP0032', 'CN01', 'NV0007', 0, 295, '2025-10-20'),
		('TK0033', 'SP0033', 'CN01', 'NV0007', 0, 455, '2025-10-20'),
		('TK0034', 'SP0034', 'CN01', 'NV0007', 0, 152, '2025-10-20'),
		('TK0035', 'SP0035', 'CN01', 'NV0007', 0, 148, '2025-10-20'),
		('TK0036', 'SP0036', 'CN01', 'NV0007', 0, 155, '2025-10-20'),
		('TK0037', 'SP0037', 'CN01', 'NV0007', 0, 225, '2025-10-20'),
		('TK0038', 'SP0038', 'CN01', 'NV0007', 0, 163, '2025-10-20'),
		('TK0039', 'SP0039', 'CN01', 'NV0007', 0, 102, '2025-10-20'),
		('TK0040', 'SP0040', 'CN01', 'NV0007', 0, 88, '2025-10-20'),
		('TK0041', 'SP0041', 'CN01', 'NV0007', 0, 118, '2025-10-20'),
		('TK0042', 'SP0042', 'CN01', 'NV0007', 0, 112, '2025-10-20'),
		('TK0043', 'SP0043', 'CN01', 'NV0007', 0, 52, '2025-10-20'),
		('TK0044', 'SP0044', 'CN01', 'NV0007', 0, 158, '2025-10-20'),
		('TK0045', 'SP0045', 'CN01', 'NV0007', 0, 285, '2025-10-20'),
		('TK0046', 'SP0046', 'CN01', 'NV0007', 0, 195, '2025-10-20'),
		('TK0047', 'SP0047', 'CN01', 'NV0007', 0, 348, '2025-10-20'),
		('TK0048', 'SP0048', 'CN01', 'NV0007', 0, 318, '2025-10-20'),
		('TK0049', 'SP0049', 'CN01', 'NV0007', 0, 298, '2025-10-20'),
		('TK0050', 'SP0050', 'CN01', 'NV0007', 0, 305, '2025-10-20'),

		-- CN02 (MaNhanVien: NV0016)
		('TK0051', 'SP0001', 'CN02', 'NV0016', 0, 55, '2025-10-21'),
		('TK0052', 'SP0002', 'CN02', 'NV0016', 0, 130, '2025-10-21'),
		('TK0053', 'SP0003', 'CN02', 'NV0016', 0, 120, '2025-10-21'),
		('TK0054', 'SP0004', 'CN02', 'NV0016', 0, 65, '2025-10-21'),
		('TK0055', 'SP0005', 'CN02', 'NV0016', 0, 190, '2025-10-21'),
		('TK0056', 'SP0006', 'CN02', 'NV0016', 0, 45, '2025-10-21'),
		('TK0057', 'SP0007', 'CN02', 'NV0016', 0, 170, '2025-10-21'),
		('TK0058', 'SP0008', 'CN02', 'NV0016', 0, 60, '2025-10-21'),
		('TK0059', 'SP0009', 'CN02', 'NV0016', 0, 105, '2025-10-21'),
		('TK0060', 'SP0010', 'CN02', 'NV0016', 0, 95, '2025-10-21'),
		('TK0061', 'SP0011', 'CN02', 'NV0016', 0, 40, '2025-10-21'),
		('TK0062', 'SP0012', 'CN02', 'NV0016', 0, 80, '2025-10-21'),
		('TK0063', 'SP0013', 'CN02', 'NV0016', 0, 95, '2025-10-21'),
		('TK0064', 'SP0014', 'CN02', 'NV0016', 0, 55, '2025-10-21'),
		('TK0065', 'SP0015', 'CN02', 'NV0016', 0, 25, '2025-10-21'),
		('TK0066', 'SP0016', 'CN02', 'NV0016', 0, 70, '2025-10-21'),
		('TK0067', 'SP0017', 'CN02', 'NV0016', 0, 120, '2025-10-21'),
		('TK0068', 'SP0018', 'CN02', 'NV0016', 0, 230, '2025-10-21'),
		('TK0069', 'SP0019', 'CN02', 'NV0016', 0, 410, '2025-10-21'),
		('TK0070', 'SP0020', 'CN02', 'NV0016', 0, 360, '2025-10-21'),
		('TK0071', 'SP0021', 'CN02', 'NV0016', 0, 95, '2025-10-21'),
		('TK0072', 'SP0022', 'CN02', 'NV0016', 0, 100, '2025-10-21'),
		('TK0073', 'SP0023', 'CN02', 'NV0016', 0, 190, '2025-10-21'),
		('TK0074', 'SP0024', 'CN02', 'NV0016', 0, 200, '2025-10-21'),
		('TK0075', 'SP0025', 'CN02', 'NV0016', 0, 45, '2025-10-21'),
		('TK0076', 'SP0026', 'CN02', 'NV0016', 0, 220, '2025-10-21'),
		('TK0077', 'SP0027', 'CN02', 'NV0016', 0, 135, '2025-10-21'),
		('TK0078', 'SP0028', 'CN02', 'NV0016', 0, 170, '2025-10-21'),
		('TK0079', 'SP0029', 'CN02', 'NV0016', 0, 175, '2025-10-21'),
		('TK0080', 'SP0030', 'CN02', 'NV0016', 0, 155, '2025-10-21'),
		('TK0081', 'SP0031', 'CN02', 'NV0016', 0, 165, '2025-10-21'),
		('TK0082', 'SP0032', 'CN02', 'NV0016', 0, 290, '2025-10-21'),
		('TK0083', 'SP0033', 'CN02', 'NV0016', 0, 440, '2025-10-21'),
		('TK0084', 'SP0034', 'CN02', 'NV0016', 0, 145, '2025-10-21'),
		('TK0085', 'SP0035', 'CN02', 'NV0016', 0, 140, '2025-10-21'),
		('TK0086', 'SP0036', 'CN02', 'NV0016', 0, 160, '2025-10-21'),
		('TK0087', 'SP0037', 'CN02', 'NV0016', 0, 230, '2025-10-21'),
		('TK0088', 'SP0038', 'CN02', 'NV0016', 0, 150, '2025-10-21'),
		('TK0089', 'SP0039', 'CN02', 'NV0016', 0, 110, '2025-10-21'),
		('TK0090', 'SP0040', 'CN02', 'NV0016', 0, 85, '2025-10-21'),
		('TK0091', 'SP0041', 'CN02', 'NV0016', 0, 130, '2025-10-21'),
		('TK0092', 'SP0042', 'CN02', 'NV0016', 0, 115, '2025-10-21'),
		('TK0093', 'SP0043', 'CN02', 'NV0016', 0, 55, '2025-10-21'),
		('TK0094', 'SP0044', 'CN02', 'NV0016', 0, 170, '2025-10-21'),
		('TK0095', 'SP0045', 'CN02', 'NV0016', 0, 270, '2025-10-21'),
		('TK0096', 'SP0046', 'CN02', 'NV0016', 0, 210, '2025-10-21'),
		('TK0097', 'SP0047', 'CN02', 'NV0016', 0, 340, '2025-10-21'),
		('TK0098', 'SP0048', 'CN02', 'NV0016', 0, 310, '2025-10-21'),
		('TK0099', 'SP0049', 'CN02', 'NV0016', 0, 295, '2025-10-21'),
		('TK0100', 'SP0050', 'CN02', 'NV0016', 0, 310, '2025-10-21'),

		-- CN03 (MaNhanVien: NV0024)
		('TK0101', 'SP0001', 'CN03', 'NV0024', 0, 65, '2025-10-21'),
		('TK0102', 'SP0002', 'CN03', 'NV0024', 0, 110, '2025-10-21'),
		('TK0103', 'SP0003', 'CN03', 'NV0024', 0, 140, '2025-10-21'),
		('TK0104', 'SP0004', 'CN03', 'NV0024', 0, 75, '2025-10-21'),
		('TK0105', 'SP0005', 'CN03', 'NV0024', 0, 210, '2025-10-21'),
		('TK0106', 'SP0006', 'CN03', 'NV0024', 0, 35, '2025-10-21'),
		('TK0107', 'SP0007', 'CN03', 'NV0024', 0, 165, '2025-10-21'),
		('TK0108', 'SP0008', 'CN03', 'NV0024', 0, 55, '2025-10-21'),
		('TK0109', 'SP0009', 'CN03', 'NV0024', 0, 115, '2025-10-21'),
		('TK0110', 'SP0010', 'CN03', 'NV0024', 0, 95, '2025-10-21'),
		('TK0111', 'SP0011', 'CN03', 'NV0024', 0, 50, '2025-10-21'),
		('TK0112', 'SP0012', 'CN03', 'NV0024', 0, 65, '2025-10-21'),
		('TK0113', 'SP0013', 'CN03', 'NV0024', 0, 110, '2025-10-21'),
		('TK0114', 'SP0014', 'CN03', 'NV0024', 0, 70, '2025-10-21'),
		('TK0115', 'SP0015', 'CN03', 'NV0024', 0, 35, '2025-10-21'),
		('TK0116', 'SP0016', 'CN03', 'NV0024', 0, 75, '2025-10-21'),
		('TK0117', 'SP0017', 'CN03', 'NV0024', 0, 105, '2025-10-21'),
		('TK0118', 'SP0018', 'CN03', 'NV0024', 0, 220, '2025-10-21'),
		('TK0119', 'SP0019', 'CN03', 'NV0024', 0, 430, '2025-10-21'),
		('TK0120', 'SP0020', 'CN03', 'NV0024', 0, 340, '2025-10-21'),
		('TK0121', 'SP0021', 'CN03', 'NV0024', 0, 110, '2025-10-21'),
		('TK0122', 'SP0022', 'CN03', 'NV0024', 0, 85, '2025-10-21'),
		('TK0123', 'SP0023', 'CN03', 'NV0024', 0, 210, '2025-10-21'),
		('TK0124', 'SP0024', 'CN03', 'NV0024', 0, 185, '2025-10-21'),
		('TK0125', 'SP0025', 'CN03', 'NV0024', 0, 45, '2025-10-21'),
		('TK0126', 'SP0026', 'CN03', 'NV0024', 0, 200, '2025-10-21'),
		('TK0127', 'SP0027', 'CN03', 'NV0024', 0, 135, '2025-10-21'),
		('TK0128', 'SP0028', 'CN03', 'NV0024', 0, 190, '2025-10-21'),
		('TK0129', 'SP0029', 'CN03', 'NV0024', 0, 175, '2025-10-21'),
		('TK0130', 'SP0030', 'CN03', 'NV0024', 0, 165, '2025-10-21'),
		('TK0131', 'SP0031', 'CN03', 'NV0024', 0, 155, '2025-10-21'),
		('TK0132', 'SP0032', 'CN03', 'NV0024', 0, 310, '2025-10-21'),
		('TK0133', 'SP0033', 'CN03', 'NV0024', 0, 460, '2025-10-21'),
		('TK0134', 'SP0034', 'CN03', 'NV0024', 0, 140, '2025-10-21'),
		('TK0135', 'SP0035', 'CN03', 'NV0024', 0, 155, '2025-10-21'),
		('TK0136', 'SP0036', 'CN03', 'NV0024', 0, 145, '2025-10-21'),
		('TK0137', 'SP0037', 'CN03', 'NV0024', 0, 210, '2025-10-21'),
		('TK0138', 'SP0038', 'CN03', 'NV0024', 0, 165, '2025-10-21'),
		('TK0139', 'SP0039', 'CN03', 'NV0024', 0, 95, '2025-10-21'),
		('TK0140', 'SP0040', 'CN03', 'NV0024', 0, 100, '2025-10-21'),
		('TK0141', 'SP0041', 'CN03', 'NV0024', 0, 125, '2025-10-21'),
		('TK0142', 'SP0042', 'CN03', 'NV0024', 0, 115, '2025-10-21'),
		('TK0143', 'SP0043', 'CN03', 'NV0024', 0, 60, '2025-10-21'),
		('TK0144', 'SP0044', 'CN03', 'NV0024', 0, 155, '2025-10-21'),
		('TK0145', 'SP0045', 'CN03', 'NV0024', 0, 290, '2025-10-21'),
		('TK0146', 'SP0046', 'CN03', 'NV0024', 0, 190, '2025-10-21'),
		('TK0147', 'SP0047', 'CN03', 'NV0024', 0, 345, '2025-10-21'),
		('TK0148', 'SP0048', 'CN03', 'NV0024', 0, 315, '2025-10-21'),
		('TK0149', 'SP0049', 'CN03', 'NV0024', 0, 305, '2025-10-21'),
		('TK0150', 'SP0050', 'CN03', 'NV0024', 0, 295, '2025-10-21'),

		-- CN04 (MaNhanVien: NV0032)
		('TK0151', 'SP0001', 'CN04', 'NV0032', 0, 50, '2025-10-22'),
		('TK0152', 'SP0002', 'CN04', 'NV0032', 0, 115, '2025-10-22'),
		('TK0153', 'SP0003', 'CN04', 'NV0032', 0, 135, '2025-10-22'),
		('TK0154', 'SP0004', 'CN04', 'NV0032', 0, 60, '2025-10-22'),
		('TK0155', 'SP0005', 'CN04', 'NV0032', 0, 180, '2025-10-22'),
		('TK0156', 'SP0006', 'CN04', 'NV0032', 0, 45, '2025-10-22'),
		('TK0157', 'SP0007', 'CN04', 'NV0032', 0, 175, '2025-10-22'),
		('TK0158', 'SP0008', 'CN04', 'NV0032', 0, 45, '2025-10-22'),
		('TK0159', 'SP0009', 'CN04', 'NV0032', 0, 120, '2025-10-22'),
		('TK0160', 'SP0010', 'CN04', 'NV0032', 0, 85, '2025-10-22'),
		('TK0161', 'SP0011', 'CN04', 'NV0032', 0, 40, '2025-10-22'),
		('TK0162', 'SP0012', 'CN04', 'NV0032', 0, 75, '2025-10-22'),
		('TK0163', 'SP0013', 'CN04', 'NV0032', 0, 105, '2025-10-22'),
		('TK0164', 'SP0014', 'CN04', 'NV0032', 0, 55, '2025-10-22'),
		('TK0165', 'SP0015', 'CN04', 'NV0032', 0, 25, '2025-10-22'),
		('TK0166', 'SP0016', 'CN04', 'NV0032', 0, 70, '2025-10-22'),
		('TK0167', 'SP0017', 'CN04', 'NV0032', 0, 100, '2025-10-22'),
		('TK0168', 'SP0018', 'CN04', 'NV0032', 0, 195, '2025-10-22'),
		('TK0169', 'SP0019', 'CN04', 'NV0032', 0, 400, '2025-10-22'),
		('TK0170', 'SP0020', 'CN04', 'NV0032', 0, 330, '2025-10-22'),
		('TK0171', 'SP0021', 'CN04', 'NV0032', 0, 95, '2025-10-22'),
		('TK0172', 'SP0022', 'CN04', 'NV0032', 0, 100, '2025-10-22'),
		('TK0173', 'SP0023', 'CN04', 'NV0032', 0, 185, '2025-10-22'),
		('TK0174', 'SP0024', 'CN04', 'NV0032', 0, 175, '2025-10-22'),
		('TK0175', 'SP0025', 'CN04', 'NV0032', 0, 35, '2025-10-22'),
		('TK0176', 'SP0026', 'CN04', 'NV0032', 0, 220, '2025-10-22'),
		('TK0177', 'SP0027', 'CN04', 'NV0032', 0, 140, '2025-10-22'),
		('TK0178', 'SP0028', 'CN04', 'NV0032', 0, 175, '2025-10-22'),
		('TK0179', 'SP0029', 'CN04', 'NV0032', 0, 185, '2025-10-22'),
		('TK0180', 'SP0030', 'CN04', 'NV0032', 0, 155, '2025-10-22'),
		('TK0181', 'SP0031', 'CN04', 'NV0032', 0, 150, '2025-10-22'),
		('TK0182', 'SP0032', 'CN04', 'NV0032', 0, 280, '2025-10-22'),
		('TK0183', 'SP0033', 'CN04', 'NV0032', 0, 430, '2025-10-22'),
		('TK0184', 'SP0034', 'CN04', 'NV0032', 0, 135, '2025-10-22'),
		('TK0185', 'SP0035', 'CN04', 'NV0032', 0, 145, '2025-10-22'),
		('TK0186', 'SP0036', 'CN04', 'NV0032', 0, 140, '2025-10-22'),
		('TK0187', 'SP0037', 'CN04', 'NV0032', 0, 215, '2025-10-22'),
		('TK0188', 'SP0038', 'CN04', 'NV0032', 0, 150, '2025-10-22'),
		('TK0189', 'SP0039', 'CN04', 'NV0032', 0, 105, '2025-10-22'),
		('TK0190', 'SP0040', 'CN04', 'NV0032', 0, 95, '2025-10-22'),
		('TK0191', 'SP0041', 'CN04', 'NV0032', 0, 115, '2025-10-22'),
		('TK0192', 'SP0042', 'CN04', 'NV0032', 0, 120, '2025-10-22'),
		('TK0193', 'SP0043', 'CN04', 'NV0032', 0, 55, '2025-10-22'),
		('TK0194', 'SP0044', 'CN04', 'NV0032', 0, 150, '2025-10-22'),
		('TK0195', 'SP0045', 'CN04', 'NV0032', 0, 270, '2025-10-22'),
		('TK0196', 'SP0046', 'CN04', 'NV0032', 0, 190, '2025-10-22'),
		('TK0197', 'SP0047', 'CN04', 'NV0032', 0, 355, '2025-10-22'),
		('TK0198', 'SP0048', 'CN04', 'NV0032', 0, 300, '2025-10-22'),
		('TK0199', 'SP0049', 'CN04', 'NV0032', 0, 290, '2025-10-22'),
		('TK0200', 'SP0050', 'CN04', 'NV0032', 0, 310, '2025-10-22'),

		-- CN05 (MaNhanVien: NV0040)
		('TK0201', 'SP0001', 'CN05', 'NV0040', 0, 50, '2025-10-23'),
		('TK0202', 'SP0002', 'CN05', 'NV0040', 0, 110, '2025-10-23'),
		('TK0203', 'SP0003', 'CN05', 'NV0040', 0, 120, '2025-10-23'),
		('TK0204', 'SP0004', 'CN05', 'NV0040', 0, 65, '2025-10-23'),
		('TK0205', 'SP0005', 'CN05', 'NV0040', 0, 190, '2025-10-23'),
		('TK0206', 'SP0006', 'CN05', 'NV0040', 0, 35, '2025-10-23'),
		('TK0207', 'SP0007', 'CN05', 'NV0040', 0, 160, '2025-10-23'),
		('TK0208', 'SP0008', 'CN05', 'NV0040', 0, 50, '2025-10-23'),
		('TK0209', 'SP0009', 'CN05', 'NV0040', 0, 100, '2025-10-23'),
		('TK0210', 'SP0010', 'CN05', 'NV0040', 0, 80, '2025-10-23'),
		('TK0211', 'SP0011', 'CN05', 'NV0040', 0, 40, '2025-10-23'),
		('TK0212', 'SP0012', 'CN05', 'NV0040', 0, 60, '2025-10-23'),
		('TK0213', 'SP0013', 'CN05', 'NV0040', 0, 95, '2025-10-23'),
		('TK0214', 'SP0014', 'CN05', 'NV0040', 0, 55, '2025-10-23'),
		('TK0215', 'SP0015', 'CN05', 'NV0040', 0, 30, '2025-10-23'),
		('TK0216', 'SP0016', 'CN05', 'NV0040', 0, 60, '2025-10-23'),
		('TK0217', 'SP0017', 'CN05', 'NV0040', 0, 100, '2025-10-23'),
		('TK0218', 'SP0018', 'CN05', 'NV0040', 0, 200, '2025-10-23'),
		('TK0219', 'SP0019', 'CN05', 'NV0040', 0, 380, '2025-10-23'),
		('TK0220', 'SP0020', 'CN05', 'NV0040', 0, 320, '2025-10-23'),
		('TK0221', 'SP0021', 'CN05', 'NV0040', 0, 90, '2025-10-23'),
		('TK0222', 'SP0022', 'CN05', 'NV0040', 0, 80, '2025-10-23'),
		('TK0223', 'SP0023', 'CN05', 'NV0040', 0, 185, '2025-10-23'),
		('TK0224', 'SP0024', 'CN05', 'NV0040', 0, 170, '2025-10-23'),
		('TK0225', 'SP0025', 'CN05', 'NV0040', 0, 35, '2025-10-23'),
		('TK0226', 'SP0026', 'CN05', 'NV0040', 0, 190, '2025-10-23'),
		('TK0227', 'SP0027', 'CN05', 'NV0040', 0, 125, '2025-10-23'),
		('TK0228', 'SP0028', 'CN05', 'NV0040', 0, 165, '2025-10-23'),
		('TK0229', 'SP0029', 'CN05', 'NV0040', 0, 170, '2025-10-23'),
		('TK0230', 'SP0030', 'CN05', 'NV0040', 0, 150, '2025-10-23'),
		('TK0231', 'SP0031', 'CN05', 'NV0040', 0, 145, '2025-10-23'),
		('TK0232', 'SP0032', 'CN05', 'NV0040', 0, 270, '2025-10-23'),
		('TK0233', 'SP0033', 'CN05', 'NV0040', 0, 420, '2025-10-23'),
		('TK0234', 'SP0034', 'CN05', 'NV0040', 0, 140, '2025-10-23'),
		('TK0235', 'SP0035', 'CN05', 'NV0040', 0, 135, '2025-10-23'),
		('TK0236', 'SP0036', 'CN05', 'NV0040', 0, 140, '2025-10-23'),
		('TK0237', 'SP0037', 'CN05', 'NV0040', 0, 200, '2025-10-23'),
		('TK0238', 'SP0038', 'CN05', 'NV0040', 0, 150, '2025-10-23'),
		('TK0239', 'SP0039', 'CN05', 'NV0040', 0, 95, '2025-10-23'),
		('TK0240', 'SP0040', 'CN05', 'NV0040', 0, 85, '2025-10-23'),
		('TK0241', 'SP0041', 'CN05', 'NV0040', 0, 110, '2025-10-23'),
		('TK0242', 'SP0042', 'CN05', 'NV0040', 0, 100, '2025-10-23'),
		('TK0243', 'SP0043', 'CN05', 'NV0040', 0, 45, '2025-10-23'),
		('TK0244', 'SP0044', 'CN05', 'NV0040', 0, 145, '2025-10-23'),
		('TK0245', 'SP0045', 'CN05', 'NV0040', 0, 260, '2025-10-23'),
		('TK0246', 'SP0046', 'CN05', 'NV0040', 0, 180, '2025-10-23'),
		('TK0247', 'SP0047', 'CN05', 'NV0040', 0, 330, '2025-10-23'),
		('TK0248', 'SP0048', 'CN05', 'NV0040', 0, 300, '2025-10-23'),
		('TK0249', 'SP0049', 'CN05', 'NV0040', 0, 280, '2025-10-23'),
		('TK0250', 'SP0050', 'CN05', 'NV0040', 0, 290, '2025-10-23'),

		-- CN06 (MaNhanVien: NV0048)
		('TK0251', 'SP0001', 'CN06', 'NV0048', 0, 50, '2025-10-24'),
		('TK0252', 'SP0002', 'CN06', 'NV0048', 0, 110, '2025-10-24'),
		('TK0253', 'SP0003', 'CN06', 'NV0048', 0, 120, '2025-10-24'),
		('TK0254', 'SP0004', 'CN06', 'NV0048', 0, 60, '2025-10-24'),
		('TK0255', 'SP0005', 'CN06', 'NV0048', 0, 180, '2025-10-24'),
		('TK0256', 'SP0006', 'CN06', 'NV0048', 0, 35, '2025-10-24'),
		('TK0257', 'SP0007', 'CN06', 'NV0048', 0, 160, '2025-10-24'),
		('TK0258', 'SP0008', 'CN06', 'NV0048', 0, 45, '2025-10-24'),
		('TK0259', 'SP0009', 'CN06', 'NV0048', 0, 100, '2025-10-24'),
		('TK0260', 'SP0010', 'CN06', 'NV0048', 0, 80, '2025-10-24'),
		('TK0261', 'SP0011', 'CN06', 'NV0048', 0, 40, '2025-10-24'),
		('TK0262', 'SP0012', 'CN06', 'NV0048', 0, 60, '2025-10-24'),
		('TK0263', 'SP0013', 'CN06', 'NV0048', 0, 95, '2025-10-24'),
		('TK0264', 'SP0014', 'CN06', 'NV0048', 0, 55, '2025-10-24'),
		('TK0265', 'SP0015', 'CN06', 'NV0048', 0, 25, '2025-10-24'),
		('TK0266', 'SP0016', 'CN06', 'NV0048', 0, 60, '2025-10-24'),
		('TK0267', 'SP0017', 'CN06', 'NV0048', 0, 95, '2025-10-24'),
		('TK0268', 'SP0018', 'CN06', 'NV0048', 0, 200, '2025-10-24'),
		('TK0269', 'SP0019', 'CN06', 'NV0048', 0, 380, '2025-10-24'),
		('TK0270', 'SP0020', 'CN06', 'NV0048', 0, 320, '2025-10-24'),
		('TK0271', 'SP0021', 'CN06', 'NV0048', 0, 90, '2025-10-24'),
		('TK0272', 'SP0022', 'CN06', 'NV0048', 0, 80, '2025-10-24'),
		('TK0273', 'SP0023', 'CN06', 'NV0048', 0, 185, '2025-10-24'),
		('TK0274', 'SP0024', 'CN06', 'NV0048', 0, 170, '2025-10-24'),
		('TK0275', 'SP0025', 'CN06', 'NV0048', 0, 35, '2025-10-24'),
		('TK0276', 'SP0026', 'CN06', 'NV0048', 0, 190, '2025-10-24'),
		('TK0277', 'SP0027', 'CN06', 'NV0048', 0, 125, '2025-10-24'),
		('TK0278', 'SP0028', 'CN06', 'NV0048', 0, 165, '2025-10-24'),
		('TK0279', 'SP0029', 'CN06', 'NV0048', 0, 170, '2025-10-24'),
		('TK0280', 'SP0030', 'CN06', 'NV0048', 0, 150, '2025-10-24'),
		('TK0281', 'SP0031', 'CN06', 'NV0048', 0, 145, '2025-10-24'),
		('TK0282', 'SP0032', 'CN06', 'NV0048', 0, 270, '2025-10-24'),
		('TK0283', 'SP0033', 'CN06', 'NV0048', 0, 420, '2025-10-24'),
		('TK0284', 'SP0034', 'CN06', 'NV0048', 0, 140, '2025-10-24'),
		('TK0285', 'SP0035', 'CN06', 'NV0048', 0, 135, '2025-10-24'),
		('TK0286', 'SP0036', 'CN06', 'NV0048', 0, 140, '2025-10-24'),
		('TK0287', 'SP0037', 'CN06', 'NV0048', 0, 200, '2025-10-24'),
		('TK0288', 'SP0038', 'CN06', 'NV0048', 0, 150, '2025-10-24'),
		('TK0289', 'SP0039', 'CN06', 'NV0048', 0, 95, '2025-10-24'),
		('TK0290', 'SP0040', 'CN06', 'NV0048', 0, 85, '2025-10-24'),
		('TK0291', 'SP0041', 'CN06', 'NV0048', 0, 110, '2025-10-24'),
		('TK0292', 'SP0042', 'CN06', 'NV0048', 0, 100, '2025-10-24'),
		('TK0293', 'SP0043', 'CN06', 'NV0048', 0, 45, '2025-10-24'),
		('TK0294', 'SP0044', 'CN06', 'NV0048', 0, 145, '2025-10-24'),
		('TK0295', 'SP0045', 'CN06', 'NV0048', 0, 260, '2025-10-24'),
		('TK0296', 'SP0046', 'CN06', 'NV0048', 0, 180, '2025-10-24'),
		('TK0297', 'SP0047', 'CN06', 'NV0048', 0, 330, '2025-10-24'),
		('TK0298', 'SP0048', 'CN06', 'NV0048', 0, 300, '2025-10-24'),
		('TK0299', 'SP0049', 'CN06', 'NV0048', 0, 280, '2025-10-24'),
		('TK0300', 'SP0050', 'CN06', 'NV0048', 0, 290, '2025-10-24');



-- Câu lệnh để tạo mục số tiền bằng chữ trong report
-- hàm đọc 3 chữ số
create function dbo.f_3so (@soba int, @isFirst bit = 0)
returns nvarchar(max)
as
begin
    declare @kq nvarchar(max) = ''
    declare @tram int = @soba / 100
    declare @chuc int = (@soba % 100) / 10
    declare @dv int = @soba % 10

    declare @chu table (so int, chu nvarchar(20))
    insert into @chu
    values (0, N'không'), (1, N'một'), (2, N'hai'), (3, N'ba'), (4, N'bốn'),
           (5, N'năm'), (6, N'sáu'), (7, N'bảy'), (8, N'tám'), (9, N'chín')

    if @tram > 0
        set @kq += (select chu from @chu where so = @tram) + N' trăm '
    else if (@chuc > 0 or @dv > 0) and @isFirst = 0
        set @kq += N'không trăm '

    if @chuc > 1
        set @kq += (select chu from @chu where so = @chuc) + N' mươi '
    else if @chuc = 1
        set @kq += N'mười '
    else if @dv > 0 and @tram > 0 and @isFirst = 0
        set @kq += N'lẻ '

    if @dv > 0
    begin
        if @dv = 1
            set @kq += case when @chuc > 1 then N'mốt' else N'một' end
        else if @dv = 4
            set @kq += case when @chuc > 1 then N'tư' else N'bốn' end
        else if @dv = 5
            set @kq += case when @chuc >= 1 then N'lăm' else N'năm' end
        else
            set @kq += (select chu from @chu where so = @dv)
    end

    return ltrim(rtrim(@kq))
end
go

-- hàm đọc tổng số tiền
create function dbo.f_sothanhchu (@sotien bigint)
returns nvarchar(max)
as
begin
    declare @ketqua nvarchar(max) = N''

    declare @ty bigint = @sotien / 1000000000
    declare @trieu int = (@sotien % 1000000000) / 1000000
    declare @nghin int = (@sotien % 1000000) / 1000
    declare @tram int = @sotien % 1000

    if @ty > 0
    begin
        set @ketqua += dbo.f_3so(@ty, 1) + N' tỷ '
    end

    if @trieu > 0
    begin
        if @ketqua = N''
            set @ketqua += dbo.f_3so(@trieu, 1) + N' triệu '
        else
            set @ketqua += dbo.f_3so(@trieu, 0) + N' triệu '
    end

    if @nghin > 0
    begin
        if @ketqua = N''
            set @ketqua += dbo.f_3so(@nghin, 1) + N' nghìn '
        else
            set @ketqua += dbo.f_3so(@nghin, 0) + N' nghìn '
    end

    if @tram > 0 or @ketqua = N''
    begin
        if @ketqua = N''
            set @ketqua += dbo.f_3so(@tram, 1)
        else
            set @ketqua += dbo.f_3so(@tram, 0)
    end

    -- viết hoa chữ cái đầu
    set @ketqua = upper(left(@ketqua, 1)) + substring(@ketqua, 2, len(@ketqua))

    return ltrim(rtrim(@ketqua)) + N' đồng'
end
go

CREATE OR ALTER VIEW view_HoaDon_ChiTiet AS
SELECT 
    hd.MaHoaDon,
    cn.TenChiNhanh,
    cn.DiaChi AS DiaChiChiNhanh,

    kh.MaKhachHang,
    kh.TenKhachHang,

    nv.MaNhanVien,
    nv.HoNhanVien + ' ' + nv.TenNhanVien AS TenNhanVien,

    sp.MaSanPham,
    sp.TenSanPham,
    sp.DonViTinh,

    ctdh.SoLuong,
    ctdh.DonGia,
    (ctdh.SoLuong * ctdh.DonGia) AS TongTien,

    hd.TongTien AS ThanhTien,
    hd.SoTienDaThanhToan,
    hd.SoDuConLai

    --hd.TongTien AS TongTienHoaDon
FROM tblHoaDon hd
JOIN tblThanhToan tt ON hd.MaThanhToan = tt.MaThanhToan
JOIN tblDonHang dh ON tt.MaDonHang = dh.MaDonHang
JOIN tblChiNhanh cn ON dh.MaChiNhanh = cn.MaChiNhanh
JOIN tblChiTietDonHang ctdh ON dh.MaDonHang = ctdh.MaDonHang
JOIN tblSanPham sp ON ctdh.MaSanPham = sp.MaSanPham
JOIN tblKhachHang kh ON dh.MaKhachHang = kh.MaKhachHang
JOIN tblNhanVien nv ON hd.MaNhanVien = nv.MaNhanVien;

CREATE OR ALTER VIEW view_HoaDon_ThongTin AS
SELECT 
    hd.MaHoaDon,
    hd.NgayXuatHoaDon,

    cn.TenChiNhanh,
    cn.DiaChi AS DiaChiChiNhanh,

    dh.LoaiDonHang,

    tt.PhuongThucThanhToan,

    kh.MaKhachHang,
    kh.HoKhachHang,
    kh.TenKhachHang,
    kh.DiaChi AS DiaChiKhachHang,
    kh.DienThoai AS DienThoaiKhachHang,

    nv.MaNhanVien,
    nv.HoNhanVien,
    nv.TenNhanVien

FROM tblHoaDon hd
JOIN tblThanhToan tt ON hd.MaThanhToan = tt.MaThanhToan
JOIN tblDonHang dh ON tt.MaDonHang = dh.MaDonHang
JOIN tblChiNhanh cn ON dh.MaChiNhanh = cn.MaChiNhanh
JOIN tblKhachHang kh ON tt.MaKhachHang = kh.MaKhachHang
JOIN tblNhanVien nv ON hd.MaNhanVien = nv.MaNhanVien;


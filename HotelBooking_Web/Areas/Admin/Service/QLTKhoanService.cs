using HotelBooking_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace HotelBooking_Web.Areas.Admin.Service
{
    public class QLTKhoanService
    {
        private DataClasses1DataContext db = new DataClasses1DataContext();


        public FunctResult<tbl_TaiKhoan> Them(string HoTen, string DiaChi, string Email,string SoDienThoai, string MatKhau,int VaiTroID)
        {
            FunctResult<tbl_TaiKhoan> rs = new FunctResult<tbl_TaiKhoan>();

            try
            {
                //cố gắng lấy ra tài khoản có email là 
                var qr = db.tbl_TaiKhoans.Where(o => o.Email == Email && (o.isDelete == null || o.isDelete == false));

                if (!qr.Any())
                {
                    //trường hợp chưa có email tồn tại
                    tbl_TaiKhoan new_obj = new tbl_TaiKhoan();
                    new_obj.HoTen = HoTen;
                    new_obj.DiaChi = DiaChi;
                    new_obj.Email = Email;
                    new_obj.SoDienThoai = SoDienThoai;
                    new_obj.MatKhau = MatKhau;
                    new_obj.VaiTroID = VaiTroID;

                    new_obj.Create_at = DateTime.Now;

                    db.tbl_TaiKhoans.InsertOnSubmit(new_obj);
                    db.SubmitChanges();

                    rs.ErrCode = EnumErrCode.Success;
                    rs.ErrDesc = "Thêm mới thành công";

                }
                else
                {
                    //trường hợp đã tồn tại lớp quản lý có mã lớp = maLopQL
                    rs.ErrCode = EnumErrCode.Existent;
                    rs.ErrDesc = "Thêm mới thất bại do đã tồn tại Email = " + Email;

                }

            }
            catch (Exception ex)
            {
                //nếu lấy ds lớp quản lý lỗi thì trả ra fail
                rs.ErrCode = EnumErrCode.Error;
                rs.ErrDesc = "Có lỗi xảy ra trong quá trình thêm mới. Chi tiết lỗi: " + ex.Message;

            }

            return rs;
        }



        public FunctResult<tbl_Phong> Sua(int id, string SoPhong, int LoaiPhongID, decimal GiaMoiDem, int SucChuaToiDa, string MoTa, string HinhAnh)
        {
            FunctResult<tbl_Phong> rs = new FunctResult<tbl_Phong>();

            try
            {
                //cố gắng lấy ra lớp quản lý có mã lớp là maLopQL
                var qr = db.tbl_Phongs.Where(o => o.PhongID == id && (o.isDelete == null || o.isDelete == false));

                if (qr.Any())
                {
                    //trường hợp lấy ra được dữ liệu lớp quản lý cần sửa
                    tbl_Phong old_obj = qr.SingleOrDefault();

                    old_obj.SoPhong = SoPhong ?? old_obj.SoPhong;
                    old_obj.LoaiPhongID = LoaiPhongID;
                    old_obj.GiaMoiDem = GiaMoiDem;
                    old_obj.SucChuaToiDa = SucChuaToiDa;
                    old_obj.MoTa = MoTa ?? old_obj.MoTa;
                    old_obj.HinhAnh = HinhAnh ?? old_obj.HinhAnh;
                    old_obj.Update_at = DateTime.Now;


                    db.SubmitChanges();


                    rs.ErrCode = EnumErrCode.Success;
                    rs.ErrDesc = "Chỉnh sửa thông tin phòng thành công";
                }
                else
                {
                    //trường hợp không tìm thấy lớp quản lý cần sửa

                    rs.ErrCode = EnumErrCode.NotExist;
                    rs.ErrDesc = "Không tìm thấy phòng cần sửa";
                }

            }
            catch (Exception ex)
            {
                //nếu lấy ds lớp quản lý lỗi thì trả ra fail
                rs.ErrCode = EnumErrCode.Error;
                rs.ErrDesc = "Có lỗi xảy ra trong quá trình chỉnh sửa dữ liệu phòng. Chi tiết lỗi: " + ex.Message;

            }

            return rs;
        }




        public PhongViewModel LayThongTinViewSua(int id)
        {
            var phong = db.tbl_Phongs.FirstOrDefault(x => x.PhongID == id);
            if (phong == null)
            {
                return null;
            }

            var loaiPhong = db.tbl_LoaiPhongs.FirstOrDefault(x => x.LoaiPhongID == phong.LoaiPhongID);
            var dsLoaiPhong = db.tbl_LoaiPhongs.Where(x => x.isDelete == null || x.isDelete == false).ToList();

            return new PhongViewModel
            {
                Phong = phong,
                LoaiPhong = loaiPhong,
                DSLP = dsLoaiPhong
            };
        }


        public FunctResult<tbl_Phong> Xoa(int id)
        {
            FunctResult<tbl_Phong> rs = new FunctResult<tbl_Phong>();
            try
            {
                var qr = db.tbl_Phongs.Where(o => o.PhongID == id && (o.isDelete == null || o.isDelete == false));
                if (qr.Any())
                {
                    tbl_Phong del_obj = qr.SingleOrDefault();
                    del_obj.isDelete = true;
                    del_obj.Delete_at = DateTime.Now;
                    db.SubmitChanges();
                    rs.ErrCode = EnumErrCode.Success;
                    rs.ErrDesc = "Xóa phòng thành công";

                }
                else
                {
                    rs.ErrCode = EnumErrCode.NotExist;
                    rs.ErrDesc = "không tìm thấy phòng cần xóa";

                }
            }
            catch (Exception ex)
            {
                rs.ErrCode = EnumErrCode.Error;
                rs.ErrDesc = "có lỗi trong quá trình xóa phòng";

            }
            return rs;
        }


        public List<vw_DanhSachTaiKhoan> Search(string query)
        {
            var list = db.vw_DanhSachTaiKhoans.Where(x => x.isDelete == null || x.isDelete == false);

            if (!string.IsNullOrEmpty(query))
            {
                list = list.Where(x => x.HoTen.Contains(query) || x.Email.Contains(query)|| x.SoDienThoai.Contains(query));
            }

            return list.ToList();


        }
    }
}
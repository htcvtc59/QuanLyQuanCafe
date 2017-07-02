create database QuanLyQuanCafe
go
use QuanLyQuanCafe
go
--Food
--Table
--FoodCategory
--Account
--Bill
--BillInfo

create table TableFood
(
id int identity primary key,
name nvarchar(200),
status nvarchar(100) default 0 --Trống|| Có người
)
go
create table Account
(
id int identity primary key,
displayname nvarchar(300),
username nvarchar(300),
password nvarchar(300) default 0,
type int  default 0 --1 admin || 0 staff
)
go
create table FoodCategory
(
id int identity primary key,
name nvarchar(200)
)
go
create table Food
(
id int identity primary key,
name nvarchar(200),
idcategory int,
price float default 0
foreign key (idcategory) references dbo.FoodCategory(id)
)
go
create table Bill
(
id int identity primary key,
datecheckin date default getdate(),
datecheckout date default getdate(),
idtable int,
status int default 0 --1 Đã thanh toán||0 chưa thanh toán
foreign key (idtable) references dbo.TableFood(id)
)
go
create table BillInfo
(
id int identity primary key,
idbill int,
idfood int,
quantity int default 0
foreign key (idbill) references dbo.Bill(id),
foreign key (idfood) references dbo.Food(id)
)
go

insert into Account values(
N'Minh',
'minh',
'123',
0),
(
N'Đức',
'duc',
'123',
1)

go
create proc USP_Login
@user nvarchar(200),
@pass nvarchar(200)
as 
begin
select * from dbo.Account where username=@user and password=@pass
end
go

declare @i int = 1
while @i<=10
begin
insert into dbo.TableFood(name) values(N'Bàn '+CAST(@i as nvarchar))
set @i=@i+1
end

go 

select * from dbo.TableFood
go
create proc USP_GetTableList
as
begin
select * from dbo.TableFood
end
exec USP_GetTableList
go
insert into dbo.FoodCategory(name) values(N'Hải sản'),
(N'Lâm sản'),(N'Nông sản'),(N'Nước')
go
insert into dbo.Food(name,idcategory,price) values
(N'Mực một nắng nước sa tế',1,120000),
(N'Nghêu hấp xả',1,50000),
(N'Dú dê nướng sữa',3,200000),
(N'Heo rừng nướng',2,150000),
(N'Cafe',4,12000),
(N'7Up',4,15000)
go
insert into dbo.Bill(datecheckin,datecheckout,idtable,status)values
(GETDATE(),null,1,0),
(GETDATE(),null,2,0),
(GETDATE(),GETDATE(),2,1),
(GETDATE(),GETDATE(),3,0)
go
insert into dbo.BillInfo(idbill,idfood,quantity)values
(1,1,2),
(1,3,4),
(1,5,1),
(2,1,2),
(2,6,2),
(3,5,2)
go


select f.name,bi.quantity,f.price,f.price*bi.quantity as totalPrice from dbo.BillInfo as bi , dbo.Bill as b , dbo.Food as f where 
bi.idbill=b.id and bi.idfood=f.id and b.status=0 and b.idtable=1

go
create proc USP_InsertBill
@idTable int
as
begin
insert into dbo.Bill(datecheckin,datecheckout,idtable,status,discount)values
(GETDATE(),null,@idTable,0,0)
end
go
create proc USP_InsertBillInfo
@idBill int,@idFood int,@quantity int
as
begin
declare @isExitsBillInfo int;
declare @foodCount int = 1;
select @isExitsBillInfo = id,@foodCount=quantity from dbo.BillInfo 
where idbill=@idBill and idfood=@idFood
if(@isExitsBillInfo>0)
begin
declare @newCount int = @foodCount+@quantity;
if(@newCount>0)
begin
update dbo.BillInfo set quantity=@foodCount+@quantity where idfood=@idFood
end
else
begin
delete dbo.BillInfo where idbill = @idBill and idfood=@idFood
end
end
else
begin
insert into dbo.BillInfo(idbill,idfood,quantity)values
(@idBill,@idFood,@quantity)
end
end

go
--Trigger
create trigger UTG_UpdateBillInfo
on dbo.BillInfo for insert,update
as
begin
declare @idBill int
select @idBill=idbill from inserted
declare @idTable int
select @idTable = idTable from dbo.Bill where id = @idBill and status=0
declare @count int
select @count=COUNT(*) from dbo.BillInfo where idbill=@idBill

if(@count>0)
update dbo.TableFood set status = N'1' where id= @idTable
else
update dbo.TableFood set status = N'0' where id= @idTable

end
go

create trigger UTG_UpdateBill
on dbo.Bill for update
as
begin
declare @idBill int
select @idBill=id from inserted
declare @idTable int
select @idTable = idTable from dbo.Bill where id = @idBill
declare @quantity int = 0 
select @quantity = COUNT(*) from dbo.Bill where idtable=@idTable and status=0
if(@quantity=0)
begin 
update dbo.TableFood set status = N'0' where id= @idTable
end
end
go

alter table dbo.Bill add discount int
go
update dbo.Bill set discount=0
go

create proc USP_SwitchTable
@idTable1 int , @idTable2 int
as
begin
declare @idFirstBill int
declare @idSecondBill int

declare @isFirstTableEmpty int = 1
declare @isSecondTableEmpty int = 1

select @idSecondBill = id from dbo.Bill where idtable= @idTable2 and status=0
select @idFirstBill = id from dbo.Bill where idtable= @idTable1 and status=0
if(@idFirstBill is null)
begin
insert into dbo.Bill(datecheckin,datecheckout,idtable,status,discount)values
(GETDATE(),null,@idTable1,0,0)
select @idFirstBill = max(id) from dbo.Bill where idtable = @idTable1 and status=0
end

select @isFirstTableEmpty=COUNT(*) from dbo.BillInfo where idbill=@idFirstBill

if(@idSecondBill is null)
begin
insert into dbo.Bill(datecheckin,datecheckout,idtable,status,discount)values
(GETDATE(),null,@idTable2,0,0)
select @idSecondBill = max(id) from dbo.Bill where idtable = @idTable2 and status=0
end

select @isSecondTableEmpty=COUNT(*) from dbo.BillInfo where idbill=@idSecondBill

select id into IDBillInfoTable from dbo.BillInfo where idbill=@idSecondBill 
update dbo.BillInfo set idbill= @idSecondBill where idbill=@idFirstBill
update dbo.BillInfo set idbill = @idFirstBill where id in (select * from IDBillInfoTable)
drop table IDBillInfoTable

if(@isFirstTableEmpty=0)
update dbo.TableFood set status=N'0' where id=@idTable2
if(@isSecondTableEmpty=0)
update dbo.TableFood set status=N'0' where id=@idTable1

end
go
alter table dbo.Bill add totalprice float
alter table dbo.Bill drop column totalPrice
go

create proc USP_GetListBillByDate
@checkIn date,@checkOut date
as
begin
select t.name as[Tên bàn],b.totalprice as [Tổng tiền],datecheckin as [Ngày vào],datecheckout as [Ngày ra],discount as [Giảm giá]
from dbo.Bill as b,dbo.TableFood as t 
where datecheckin>=@checkIn and datecheckout<=@checkOut and b.status=1 and t.id=b.idtable
end
go

create proc USP_UpdateAccount
@username nvarchar(200),@displayname nvarchar(200),@password nvarchar(200),@newpassword nvarchar(200)
as
begin
declare @isrightpass int=0
select @isrightpass = COUNT(*) from dbo.Account where username =@username and password = @password
if(@isrightpass=1)
begin
if(@newpassword=null or @newpassword ='')
update dbo.Account set displayname =@displayname where username = @username
else 
update dbo.Account set displayname =@displayname , password=@newpassword where username = @username
end
end
go
create proc USP_GetListFood
as
begin
select id as [Mã],name as [Tên món ăn],idcategory as [Mã danh mục],price as [Đơn giá] from dbo.Food
end
go
create trigger UTG_DeleteBillInfo
on dbo.BillInfo for delete
as 
begin
declare @idBillInfo int
declare @idBill int
select @idBillInfo = id ,@idBill = deleted.idbill from deleted

declare @idTable int
select @idTable = idtable from dbo.Bill where id=@idBill

declare @count int =0
select @count = COUNT(*)from dbo.BillInfo as bi ,dbo.Bill as b 
where b.id=bi.idbill and b.id= @idBill and b.status=0
if(@count=0)
update dbo.TableFood set status = N'0' where id=@idTable
end

go

create proc USP_SearchFoodByName
@names nvarchar(300)
as
begin
select id as [Mã],name as [Tên món ăn],idcategory as [Mã danh mục],price as [Đơn giá] from dbo.Food where name=@names
end
go
CREATE FUNCTION [dbo].[fuConvertToUnsign1] 
( @strInput NVARCHAR(4000) )
 RETURNS NVARCHAR(4000) 
 AS BEGIN IF @strInput IS NULL 
 RETURN @strInput IF @strInput = '' RETURN @strInput 
 DECLARE @RT NVARCHAR(4000) 
 DECLARE @SIGN_CHARS NCHAR(136) 
 DECLARE @UNSIGN_CHARS NCHAR (136)
  SET @SIGN_CHARS = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệế ìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵý ĂÂĐÊÔƠƯÀẢÃẠÁẰẲẴẶẮẦẨẪẬẤÈẺẼẸÉỀỂỄỆẾÌỈĨỊÍ ÒỎÕỌÓỒỔỖỘỐỜỞỠỢỚÙỦŨỤÚỪỬỮỰỨỲỶỸỴÝ' +NCHAR(272)+ NCHAR(208) 
  SET @UNSIGN_CHARS = N'aadeoouaaaaaaaaaaaaaaaeeeeeeeeee iiiiiooooooooooooooouuuuuuuuuuyyyyy AADEOOUAAAAAAAAAAAAAAAEEEEEEEEEEIIIII OOOOOOOOOOOOOOOUUUUUUUUUUYYYYYDD' 
  DECLARE @COUNTER int 
  DECLARE @COUNTER1 int 
  SET @COUNTER = 1 WHILE (@COUNTER <=LEN(@strInput))
   BEGIN SET @COUNTER1 = 1 WHILE (@COUNTER1 <=LEN(@SIGN_CHARS)+1) 
   BEGIN IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1,1)) = UNICODE(SUBSTRING(@strInput,@COUNTER ,1) )
    BEGIN IF @COUNTER=1 SET @strInput = SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)-1)
	 ELSE SET @strInput = SUBSTRING(@strInput, 1, @COUNTER-1) +SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)- @COUNTER) 
	 BREAK END SET @COUNTER1 = @COUNTER1 +1 END SET @COUNTER = @COUNTER +1 END SET @strInput = replace(@strInput,' ','-') RETURN @strInput END
	 
	 go

select * from dbo.Food
select * from dbo.TableFood
select * from dbo.FoodCategory
select * from dbo.Bill 
select * from dbo.BillInfo 
select * from dbo.Account 

go
create proc USP_GetListBillByDateAndPage
@checkIn date,@checkOut date, @page int
as
begin
declare @pageRows int = 3
declare @selectRows int = @pageRows*@page
declare @exceptRows int = (@page-1)*@pageRows

;with BillShow AS( select b.id, t.name as[Tên bàn],b.totalprice as [Tổng tiền],datecheckin as [Ngày vào],datecheckout as [Ngày ra],discount as [Giảm giá]
from dbo.Bill as b,dbo.TableFood as t 
where datecheckin>=@checkIn and datecheckout<=@checkOut and b.status=1 and t.id=b.idtable)
select top (@pageRows) * from BillShow where id not in(select top(@exceptRows) id from BillShow)
end 
go


create proc USP_GetNumBillByDate
@checkIn date,@checkOut date
as
begin
select COUNT(*)
from dbo.Bill as b,dbo.TableFood as t 
where datecheckin>=@checkIn and datecheckout<=@checkOut and b.status=1 and t.id=b.idtable
end
go

exec USP_GetListBillByDateAndPage @checkIn ='2017-06-24',@checkOut ='2017-06-24', @page =1
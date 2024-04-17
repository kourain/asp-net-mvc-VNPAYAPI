<h1 align="center">
Hướng dẫn sử dụng
</h1>
## Cho mình xin 1* nếu các bạn thích và sử dụng nó, cảm ơn

Sử dụng ```tmnCode``` và ```hashSecret``` mà VNPAY cấp cho các bạn, cổng phụ thuộc vào trang web(với đồ án chạy trên máy cá nhân xem ## Lấy cổng truy cập

![image](https://github.com/kourain/VNPAYAPI/assets/85356599/12ddf7d9-4928-42d7-a007-bd3bfda80287)

chỉnh sửa các đường dẫn phản hồi về trang web của các bạn

![image](https://github.com/kourain/VNPAYAPI/assets/85356599/8c9bde8f-4a21-48dc-87db-b1d2b8a94046)

ví dụ:

![image](https://github.com/kourain/VNPAYAPI/assets/85356599/fccf22b1-15fb-4d00-a12f-ac7ae1e1a930)

## Lấy cổng truy cập

mình sử dụng IIS Express, ví dụ cổng mình đang sử dụng với đồ án cá nhân là ``44311``

returnUrl của mình có dạng ```https://localhost:44311/vnpayAPI/PaymentConfirm```

![image](https://github.com/kourain/VNPAYAPI/assets/85356599/e9e8d69a-c892-4429-b686-1cec0b884c89)

## Đính kèm code vào chung với trang web

Các bạn chỉ cần để vào Areas

![image](https://github.com/kourain/VNPAYAPI/assets/85356599/fa95393e-a0d0-4933-b239-5c535525a4a9)

tại Program.cs các bạn bổ sung

![image](https://github.com/kourain/VNPAYAPI/assets/85356599/10a0447f-1280-4388-82d0-2f1809018e84)

``` 
app.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    ); ```

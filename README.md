# Skor Tablosu API

ASP.NET Core ile yazılmış, kullanıcıların kayıt olup kendi skorlarını ekleyebildiği, güncelleyebildiği ve silebildiği basit bir web uygulaması. Arka uçta C# API, ön yüzde HTML ve JavaScript var.

## Özellikler

- Kullanıcı kaydı ve giriş (JWT token ile), şifreler hash'lenerek saklanır
- Skorları herkes görebilir, ekleme/güncelleme/silme için giriş gerekir
- Herkes sadece kendi skorunu güncelleyebilir ve silebilir
- Id'ye göre tek skor getirme ve en yüksek skoru getirme
- Veriler SQLite veritabanında saklanır (Entity Framework Core ile)
- Sunucu tarafında doğrulama (validation)
- Sayfada satır içinde güncelleme ve hata mesajı gösterme
- XSS'e karşı güvenli listeleme (kullanıcı verisi HTML olarak yorumlanmaz)

## Doğrulama kuralları

- Oyuncu adı boş olamaz ve en fazla 30 karakter olabilir
- Puan negatif olamaz
- Kullanıcı adı boş olamaz, en fazla 30 karakter olabilir ve benzersiz olmalı
- Şifre en az 6 karakter olmalı

Kurallara uymayan istekler `400 Bad Request` ve bir hata mesajıyla reddedilir.

## Kullanılan teknolojiler

- C# / ASP.NET Core (Minimal API)
- Entity Framework Core
- SQLite
- JWT (JSON Web Token) ile kimlik doğrulama
- HTML, JavaScript

## Nasıl çalıştırılır?

1. Projeyi indir: `git clone https://github.com/ereningoai-cloud/skor-tablosu-api.git`
2. Visual Studio ile `IlkApi.slnx` dosyasını aç
3. Gizli anahtarı ayarla (token imzası için, depoda yok): proje adına sağ tıkla → **Manage User Secrets** ve şunu yaz (uzun, rastgele bir yazı kullan):
   `{ "Jwt:Key": "en-az-32-karakterlik-rastgele-bir-yazi" }`
4. F5 ile çalıştır
5. Tarayıcıda açılan sayfada kayıt ol, giriş yap ve skor ekle

## Endpoint'ler

| Yöntem | Adres | Açıklama |
|---|---|---|
| GET | /skorlar | Tüm skorları getirir |
| GET | /skorlar/{id} | Id'si verilen skoru getirir |
| GET | /skorlar/en-yuksek | En yüksek puanlı skoru getirir |
| POST | /skorlar | Yeni skor ekler (giriş gerekir) |
| PUT | /skorlar/{id} | Skoru günceller (giriş gerekir, sadece kendi skorun) |
| DELETE | /skorlar/{id} | Skoru siler (giriş gerekir, sadece kendi skorun) |
| POST | /kayit | Yeni kullanıcı kaydı |
| POST | /giris | Giriş yapar, token döner |
| GET | /ben | Giriş yapan kullanıcıyı gösterir (token gerekir) |
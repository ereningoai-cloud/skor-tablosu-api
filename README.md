# Skor Tablosu API

ASP.NET Core ile yazılmış, skorları ekleyen, listeleyen, güncelleyen ve silen basit bir web uygulaması. Arka uçta C# API, ön yüzde HTML ve JavaScript var.

## Özellikler

- Tam CRUD: skor ekleme, listeleme, güncelleme, silme
- Id'ye göre tek skor getirme ve en yüksek skoru getirme
- Veriler SQLite veritabanında saklanır (Entity Framework Core ile)
- Sunucu tarafında doğrulama (validation)
- Sayfada satır içinde güncelleme ve hata mesajı gösterme
- XSS'e karşı güvenli listeleme (kullanıcı verisi HTML olarak yorumlanmaz)

## Doğrulama kuralları

- Oyuncu adı boş olamaz
- Oyuncu adı en fazla 30 karakter olabilir
- Puan negatif olamaz

Kurallara uymayan istekler `400 Bad Request` ve bir hata mesajıyla reddedilir.

## Kullanılan teknolojiler

- C# / ASP.NET Core (Minimal API)
- Entity Framework Core
- SQLite
- HTML, JavaScript

## Nasıl çalıştırılır?

1. Projeyi indir: `git clone https://github.com/ereningoai-cloud/skor-tablosu-api.git`
2. Visual Studio ile `IlkApi.slnx` dosyasını aç
3. F5 ile çalıştır
4. Tarayıcıda açılan adreste skor ekleyebilirsin

Not: Geliştirme aşamasında veritabanı her açılışta sıfırlanır.

## Endpoint'ler

| Yöntem | Adres | Açıklama |
|---|---|---|
| GET | /skorlar | Tüm skorları getirir |
| GET | /skorlar/{id} | Id'si verilen skoru getirir |
| GET | /skorlar/en-yuksek | En yüksek puanlı skoru getirir |
| POST | /skorlar | Yeni skor ekler |
| PUT | /skorlar/{id} | Skoru günceller |
| DELETE | /skorlar/{id} | Skoru siler |
| DELETE | /skorlar | Tüm veritabanını sıfırlar (geliştirme için) |
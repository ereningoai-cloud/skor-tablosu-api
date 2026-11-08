# Skor Tablosu API

ASP.NET Core ile yazılmış, oyuncuların kayıt olup oyundan skor gönderdiği ve skor tablosunun web sayfasında görüldüğü bir uygulama. Arka uçta C# API, ön yüzde HTML, CSS ve JavaScript var. Skorlar bir Unity oyunundan (balon patlatma tipi bir aim trainer) gönderilir.

## Özellikler

- Kullanıcı kaydı ve giriş (JWT token ile), şifreler hash'lenerek saklanır
- Her oyuncunun tabloda tek satırı vardır ve **en iyi skoru** saklanır
- Skor sadece giriş yapmış oyuncudan gelir, oyuncu adı token'dan okunur (başkası adına skor gönderilemez)
- Web sayfasında skor tablosu giriş yapıldıktan sonra görünür
- Sıralama seçenekleri: en yeni, yüksekten düşüğe, düşükten yükseğe
- Veriler SQLite veritabanında saklanır (Entity Framework Core ile)
- XSS'e karşı güvenli listeleme (kullanıcı verisi HTML olarak yorumlanmaz)

## Kurallar

- Kullanıcı adı boş olamaz, en fazla 30 karakter olabilir ve benzersiz olmalı
- Şifre en az 6 karakter olmalı
- Puan negatif olamaz

Kurallara uymayan istekler `400 Bad Request`, var olan kullanıcı adıyla kayıt `409 Conflict`, olmayan oyuncu `404 Not Found`, yanlış şifre `401 Unauthorized` ile reddedilir.

## Kullanılan teknolojiler

- C# / ASP.NET Core (Minimal API)
- Entity Framework Core
- SQLite
- JWT (JSON Web Token) ile kimlik doğrulama
- HTML, CSS, JavaScript
- Unity (oyun istemcisi, ayrı projede)

## Nasıl çalıştırılır?

1. Projeyi indir: `git clone https://github.com/ereningoai-cloud/skor-tablosu-api.git`
2. Visual Studio ile `IlkApi.slnx` dosyasını aç
3. Gizli anahtarı ayarla (token imzası için, depoda yok): proje adına sağ tıkla → **Manage User Secrets** ve şunu yaz (uzun, rastgele bir yazı kullan):
   `{ "Jwt:Key": "en-az-32-karakterlik-rastgele-bir-yazi" }`
4. F5 ile çalıştır
5. Tarayıcıda açılan sayfada kayıt ol ve giriş yap, skor tablosu giriş sonrası görünür

Veritabanı dosyası (`skorlar.db`) ilk çalıştırmada otomatik oluşur ve depoya dahil değildir. Skorlar sadece oyundan gönderilir, web sayfasından skor eklenmez.

## Endpoint'ler

| Yöntem | Adres | Açıklama |
|---|---|---|
| GET | /skorlar | Tüm skorları getirir |
| GET | /skorlar/{id} | Id'si verilen skoru getirir |
| GET | /skorlar/en-yuksek | En yüksek puanlı skoru getirir |
| POST | /skorlar/gonder | Giriş yapan oyuncunun puanını gönderir, sadece rekor kırılırsa kaydeder (giriş gerekir) |
| POST | /kayit | Yeni kullanıcı kaydı |
| POST | /giris | Giriş yapar, token döner |
| GET | /ben | Giriş yapan kullanıcıyı gösterir (giriş gerekir) |
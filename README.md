\# Skor Tablosu API



ASP.NET Core ile yazılmış, skorları kaydeden ve listeleyen basit bir web uygulaması.



\## Özellikler



\- Skor ekleme ve listeleme

\- Id'ye göre tek skor getirme

\- En yüksek skoru getirme

\- Veriler SQLite veritabanında kalıcı olarak saklanır

\- HTML/JavaScript ile yazılmış basit bir web arayüzü



\## Kullanılan teknolojiler



\- C# / ASP.NET Core (Minimal API)

\- Entity Framework Core

\- SQLite

\- HTML, JavaScript



\## Nasıl çalıştırılır?



1\. Projeyi indir: `git clone https://github.com/ereningoai-cloud/skor-tablosu-api.git`

2\. Visual Studio ile `IlkApi.slnx` dosyasını aç

3\. F5 ile çalıştır

4\. Tarayıcıda açılan adreste skor ekleyebilirsin



\## Endpoint'ler



| Yöntem | Adres | Açıklama |

|---|---|---|

| GET | /skorlar | Tüm skorları getirir |

| GET | /skorlar/{id} | Id'si verilen skoru getirir |

| GET | /skorlar/en-yuksek | En yüksek puanlı skoru getirir |

| POST | /skorlar | Yeni skor ekler |


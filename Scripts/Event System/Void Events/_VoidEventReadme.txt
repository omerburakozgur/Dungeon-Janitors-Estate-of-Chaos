Ömer Burak Özgür tarafýndan yapýldý ve yazýldý. 5.11.2025

Void Event Channel (Kodsuz Yaklaþým)
1-Event için 
	1.1- Scriptable object eventi için yeni bir SO türet. (onTrashCleanedChannel)

2-SO event raiser
	3.1-onTrashCleanedChannel referansýný mantýk kodunun çalýþacaðý scripte referans olarak ekleyelim.
	3.2-Mantýk kodunun çalýþacaðý yerde onTrashCleanedChannel.Raise() þeklinde eventi tetikleyelim.
	3.3-Örnek olarak oyuncu baþarýlý þekilde çöp topladýðý zaman çalýþan metodun içerisine eklersek bu sinyali yayýnlayacak.
	Bu sayede VoidEventChannelSO içerisindeki sabit olarak tanýmlý olan OnEventRaised UnityEvent çalýþacak.

3-VoidEventListener üzerinden EventChannel alanýna oluþturduðumuz eventin SO referansýný verelim.
	(Bu SO referansýna sahip void event listener referansý sadece bu SO kanalýna yayýn yapacak)

4-UnityEvent'e inspector üzerinden çalýþtýrýlacak kodlarý referans olarak eklemek.
	4.1-OnEventRaised listesine + tuþuna basarak sadece runtime sýrasýnda çalýþacak referanslar oluþturalým.
	4.2-Çalýþacak kodun objesini yeni oluþturduðumuz referansýn object kýsmýna sürükleyelim.
	4.3-Function kýsmýndan çalýþtýrmak istediðimiz kod metodunu seçelim. (Public olmak zorunda yoksa inspectorda gözükmez)
	4.4-Eklemek istediðimiz her metod için tekrarlayalým.

Sonuç olarak spesifik bir kanal oluþturup bunun üzerinden kodsuz þekilde inspector aracýlýðýyla referans saðlayýp 
belli objeleri yönetmemizi ve mantýk iþlemlerini çalýþtýrmamýz için bir yöntem.








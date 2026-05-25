Ömer Burak Özgür tarafýndan yapýldý ve yazýldý. 5.11.2025

Generic Event Channel
1-Event Channel
	1.a-Eksik bir event channel var ise o tipte yeni bir event channel oluþtur.
	1.b-Uygun bir event channel var ise onu kullan.

2-Seçilmiþ event channel üzerinden yeni bir scriptable object (Örn. onSalvageUpdatedChannel) oluþtur.

3-Kod çalýþtýrma
	3.1-Mantýk kodunun çalýþacaðý script üzerinden oluþturulmuþ event SO'ya referans oluþtur (Örn. PlayerInventory).
	3.2-Eventin tetiklenmesi gereken yerde scriptable objectin Raise(variable) metodunu çalýþtýr. (onSalvageUpdatedChannel.Raise(int))

4-Listener Tepkisi
	4.1-Listener scripti üzerinden (Örn. UIManager) abone olunacak eventin scriptable objectine referans oluþtur (onSalvageUpdatedChannel).
	4.2-Bu event gerçekleþtiði zaman çalýþtýrýlacak metod ile OnEnable'da abone ol, OnDisable'da abonelikten çýk.
		(onSalvageUpdatedChannel.OnEventRaised += UpdateSalvageText, onSalvageUpdatedChannel.OnEventRaised -= UpdateSalvageText)

Not: Bu generic event scriptable object yöntemi ile event tetiklendiði zaman SO tipine göre bir veri gönderilir, evente abone olan metodun bu deðiþkeni almasý gerekir.
	
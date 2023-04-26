# MultiplayerGamePrototype
 Multiplayer framework developments.



Unity : 2021.3.23f1
Multiplayer : Unity Gaming Services


Oyun kurmak:   
    + Ana ekranda "username" boş bırakılamaz. Oyun alanındaki oyuncu stats listesinde bu isim kullanıldığı için böyle bir zorunluluk eklendi.
    + Username girildikten sonra "Create Game" butonu ile oyun kurulur.
    + Host oyun alanına girdiğinde hedefler sahnede oluşturulur.
    + Oyunu kuran kişi Host player olur ve eğer oyunda çıkarsa diğer oyuncular içinde oyun sonlanır. Oyunun sonlanması, bitiş ekranları ve oyun alanından atmak gibi eylemler eklenmedi.
    

Kurulu oyuna giriş yapmak:   
    + Diğer oyuncular Host'un kurduğu oyuna "Lobby Code" ile giriş yapabilirler.
    + "Lobby Code" oyun alanının üstündeki başlıkta "Id" numarasının altında  "Code:123456" şeklinde harf ve rakamdan oluşan metindir. 
    + Giriş ekranındaki "Join Game" butonunun yanındaki text alanına "Lobby Code"  yazılır "Join Game" butonuna tıklayarak oyuna girilir.
    + "Lobby Code" case-sensetive değildir.

Oyun ayarları : 
    + Assets => MultiplayerGamePrototype => Datas klasörü içerisindeki "GameData" objesinde  oyun ayarları ile ilgili düzenlemeleri yapabilirsiniz.
    + Score verisinde değerler Id bazlı tutulduğu için, oyundaki herhangi bir oyuncu oyunu kapatıp tekrar aynı oyuna aynı veya faklı bir kullanıcı adı ile girerse, skor listesine ayrıca ekleme yapılmaz. Oyuncunun aldığı/kaybettiği puanlar yine aynı veri üzerine yazılır. 
    Çünkü skor verisi oyun datası içerisinde Id bazlı tutuluyor ve oyun Host tarafından kapatılana kadar kalıyor. 

Oyuncu Kontrolleri:
    + Mobil ve Desktop için kontroller eklendi.
    + Keyboard F : Ateş etme
    + Keyboard B : İlk basılma anında "Stun bomb" eklemesi yapıyor ardında 5 saniye boyunca buton tıklamalara kapatılıyor, bu süre geçtikten sonra tekrar tıklanırsa mevcut bomba patlatılıyor.
    + Keyboard M : Editör ve Desktop için kaybolan mouse ikonunu geri geliyor ve bu sayede UI buton etkileşimleri yapılabiliniyor.


Platformlar:
    Android ve Mac için build alınabilir, diğer platformlar için herhangi bir build alma denemesi olmadı.


Genel:
    + Oyun için "Max Players" değerini 10 ile sınırladım sanırım bir oyun için yeterli olur.
    + Yukarı bahsettiğim gibi skor verisi oyun datası içinde tutuluyor. Ancak daha sonradan fark ettiğim bir sınırlamadan ötürü hatalı gösterim olabilme ihtimali mevcut. Skor verisi için oyuncu oyun datasına 5 saniye içinde en fazla 5 istek gönderebilir. Yani 5 saniye içinde 5 den fazla hedef vurulursa fazla olan veriler kişinin skor verisine eklenemiyor. Zamandan kaynaklı bu kısım için kontrol ekleyemedim. Ayrıca skor verisini oyun datası yerine kişinin kendi datasında tutulabilirdi ancak bu da kişinin oyundan çıkarsa listeden silinip önceki verilerin kaybolmasına sebep olabilirdi. Hem zaman hem de yapıdan ötürü bu kısmı bu şekilde bırakıldı. 
    + Zamandan kaynaklı sadece istene işlevleri yerine getirebildi.
    + Host masadan çıkarsa oyun diğer oyuncular için sonlanıyor, Host geçişi ve oyunun kalan oyuncular ile devam etmesi eklenebilirdi. 
    + Oyundan çıkma, bağlantı kopması ve sonarsında Reconnect olup yeniden oyuna girilmesi ile ilgili düzenlemeler eklenebilirdi.
    + Network kısmı için "Object pool" yapısı desteklenmesine karşın daha önce kullanmadığım için projeye eklemedim.
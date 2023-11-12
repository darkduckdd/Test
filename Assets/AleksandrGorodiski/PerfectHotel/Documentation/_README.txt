
Important! Before play the Game you need to import DOTween:
https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676



Please read "EXPLANATORY_DOC" in Docs folder before and with notes below.

Key notes:
0. Reception and Elevator are mandatory for every Hotel.
1. Every Hotel consists of Areas.

2. Area numbers should start with number one(1).
3. Area number one(1) is enabled by default. Areas number two(2), three(3), four(4) etc. needs to be purchased while play.

4. Room numbers should start with number zero(0).
5. Room number zero(0) is enabled by default. Rooms number two(2), three(3), four(4) etc. needs to be purchased while play.

6. After Customer(Unit) left Room, system will check if there is available ToiletCabine. If YES - Customer will go there. If NOT - leave Hotel.
7. System will check only for ToiletCabine with the same Area number that has Room, Customer was in.

8. Cleaner cleans only Rooms with which they has the same Area number.

9. Every Hotel has Level. Level by default is one(1).
10. Every Hotel has Progress value and MaxProgress. Progress by default is zero(0).
11. MaxProgress value for each Level are getting from Rooms and Toilets parameter "PurchaseProgressReward" and "UpdateProgressReward", that belong to Area with Lvl parameter equal to Level.

12. By purchasing(upgrading) Room or Toilet, paremeter "PurchaseProgressReward" or "UpdateProgressReward" values will be added to the Progress.
13. Once Progress equal or more then MaxProgress, Level encreased by 1.
14. When Level changed, Area which can be purchased at that Level is becoming available to purchase.

15. For Room number zero(0), parameter "PurchaseProgressReward" must be zero(0).
Since this Room is available by default and this value can never be added to the Progress value.

16. Elevator will swith to "ElevatorNoHotelSceneState" state if there is no next Hotel in Build Settins.
For example: your current Hotel is one(scene index = 1 in Build Settings) and you dont have scene with index = 2.



There is very detailed tutorial of how to add new Hotel:

Perfect Hotel. Add New Hotel. Part 1. Create Entities
https://youtu.be/Vm-xTl3Q2Fw

Perfect Hotel. Add New Hotel. Part 2. Add Entities
https://youtu.be/sQ_woJMW9Pc


It is recommended to download and have it offline:
https://drive.google.com/file/d/1qeaJEQwkoo8ROxRsW1K6v6bJwjLbx9S6/view?usp=share_link
https://drive.google.com/file/d/1qeT2kMJ0xOLXvLSrsi2ddKaDeOgdLAH5/view?usp=share_link


Game has GameConfig(Resources/GameConfig).

Some of them are:
0. "CashPileRadius", "ReceptionItemRadius" and other radiuses - distance to the correspondent items, Player can start interact with.
1. "ToiletPaperFlyTime" - time toilet paper flies from Player to the ToiletCabine.
2. "ToiletVisitsCountMax" - number of visits to the ToiletCabine before it needs to be refilled with toilet paper.
3. "InventoryMax" - maximum amount of inventory Player can carry.



Contact me if you have any question:
aleksandr.gorodiski@gmail.com

P.S. please send the purchase invoice number in your email
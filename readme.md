SUPPORT TIBIA CLIENT 13.32.14

Whatsapp
https://chat.whatsapp.com/C4OSuvnUIa55ZSIRgJ9Pib


Bot free. Versão 1.0.1.

Features

Anti Paralyze
Dash Speed
Percent HP 
Percent MP

Ajustes no cave bot..


Atenção, para o AutoAttack funcionar, é preciso configurar hotkey de ataque como o número 0.


![Attack](https://github.com/LucasDuca/ducabot/blob/main/atk.png?raw=true)

![Spells](https://github.com/LucasDuca/ducabot/blob/main/2024-05-13_23h36_19.png?raw=true)

<img src="https://github.com/LucasDuca/ducabot/blob/main/2024-05-13_23h37_26.png?raw=true" alt="Spells">

Modo de uso, se logar no tibia e abrir o executável Duca_Bot3.0.
Ele reconhecerá o primeiro cliente de tibia aberto.
Versão inicial. Bot 100% FREE.
Contact: lucasducaster@gmail.com




Parte técnica, para devs que estão tentando fazer bot.
Alguns offsets que descobri para alegria de vocês:

offsets_hp = { (IntPtr)0x1E4, (IntPtr)0x40, (IntPtr)0x18, (IntPtr)0x38, (IntPtr)0x18, (IntPtr)0x8, (IntPtr)0x50 }; //offset do HP
offsets_mp = { (IntPtr)0x1E4, (IntPtr)0x40, (IntPtr)0x18, (IntPtr)0x38, (IntPtr)0x18, (IntPtr)0x0, (IntPtr)0xc50 }; //offset do MP
offsets_posx = { (IntPtr)0xFC, (IntPtr)0x70, (IntPtr)0xC, (IntPtr)0x14, (IntPtr)0x13C, (IntPtr)0x8c, (IntPtr)0x64 }; //offset posX
offsets_posy = { (IntPtr)0xFC, (IntPtr)0x70, (IntPtr)0xC, (IntPtr)0x14, (IntPtr)0x13C, (IntPtr)0x8c, (IntPtr)0x68 }; //offset posy
offsets_posz = { (IntPtr)0xFC, (IntPtr)0x70, (IntPtr)0xC, (IntPtr)0x14, (IntPtr)0x13C, (IntPtr)0x8c, (IntPtr)0x6c }; //offset posz

Fórmula para conseguir acessar o ponteiro, utilizar o "THREADSTACK0"-110 no cheat engine e adicionar os offsets um a um, sem o 0x.
Já na programação, utilizando o handle do tibia, pegar a primeira Thread do processo e o endereço do TEB (thread enviroment block) - 0x110.

OBS: por algum motivo estranho tive que pegar o TEB  + 0x1f0, antes de calcular o 0x110- para chegar no resultado esperado :)

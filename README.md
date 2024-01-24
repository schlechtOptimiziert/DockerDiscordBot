# Installieren des Raspi image
## Raspi config
1. Raspi mit nem 64 bit light image beladen.
2. ssh bei der Installertaion auf der SD freischalten
3. Benutzer habe ich auf (admin, admin) konfiguriert

# Raspi initial einstellen
## PuTTY
1. Per ssh auf Raspi verbinden. Bei mir im Netzwerk hat der Raspi die feste IP (192.168.178.2). Port bleibt auf 22.
2. Einloggen mit vorherigem Nutzer und Password

## Einstellen
1. "sudo raspi-config" in PuTTY eingeben
2. Password ändern: System Optionen -> Password (Hint: bei PuTTY ist paste Rechtsklick)
3. Update: Update Option
4. Ganz SD verfügbar machen: Advanced Options -> Expand Filesystem
5. Finish: Finifh Option
6. Reboot: yes

## Nochmal Update
1. "sudo apt update"
2. "sudo apt upgrade" und "Y" bei der Bestätigungsfrage
3. "sudo rpi-update" und "Y" bei der Bestätigungsfrage
4. "sudo reboot"

## Mount harddrive
- Steps von: https://thepihut.com/blogs/raspberry-pi-tutorials/how-to-mount-an-external-hard-drive-on-the-raspberry-pi-raspian
1. "sudo fdisk -l" um den Namen des Anschlusses zu finden (in meinem Fall /dev/sda1)
2. "sudo mount <Anschluss> /mnt"
3. "sudo chmod 775 /mnt" Rechte auf dem Volumen zu bekommen
4. "sudo nano /etc/fstab"
5. Add Line "<Anschluss>  /mnt  <filesystem>  defaults  0  0" (Hint: zwischen den Einträgen Tabs. In meinem Fall ist das Filesystem ntfs)

# Docker
## Insterlieren
- Steps von: https://docs.docker.com/engine/install/debian/
1. "curl -fsSL https://get.docker.com -o get-docker.sh"
2. "sudo sh get-docker.sh"

## Nutzer Rechte auf Docker geben
1. "sudo usermod -aG docker $USER"

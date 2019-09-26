我們的project在 project/裡
------------------------------------------
-GIT
步驟
1.開啟atm926資料夾，在裡面用終端機打"docker build -t atm ."
2.建完後打"docker run -d --rm -p 8000:80 atm"
3.打開localhost:8000/api/atm 看是否抓取到資料庫
------------------------------------------
-DOCKER
步驟
1."docker pull chiasheng/atm"
2."docker run -d --rm -p 8000:80 chiasheng/atm"
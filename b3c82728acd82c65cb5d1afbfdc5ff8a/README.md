# NOTE

See https://gist.github.com/cerebrate/d40c89d3fa89594e1b1538b2ce9d2720#gistcomment-3563688 below before doing anything else.

# Recompile your WSL2 kernel - support for snaps, apparmor, lxc, etc.

Yes, I've done this, and yes, it works. It is, however, entirely unsupported and assembled through reasonable guesswork, so if you
try this and it explodes your computer, brain, career, relationships, or anything else, you agree that you take sole responsibility
for doing it, that I never claimed it was a good idea, and that you didn't get these instructions from _me_ .

Also note: I have done this with Debian Stretch. While one kernel ought to fit all, some of the packages you need to build it may be
different. Adapting to other distros is up to you.

## Step One: Install the stuff you need to build the kernel

```
sudo apt install build-essential flex bison libssl-dev libelf-dev
```

If you're going to tinker with the kernel config file (and why would you be doing this if you didn't want to tinker with the kernel
config, you may wish to use the friendly X-based config mechanism. If so, also do this:

```
sudo apt install qt5-default pkg-config
```

## Step Two: Clone the latest kernel source

```
git clone https://github.com/microsoft/WSL2-Linux-Kernel.git
```

## Step Three: Make sure you are compiling it the Microsoft way

Which is to say, first, clone their config so you don't leave anything out. In the folder you just cloned the kernel into:

```
cp Microsoft/config-wsl .config
```

## Step Four: Edit the configuration

Pick your method, any one of:

```
nano .config
make menuconfig
make xconfig
```

Myself, I like the graphical configuration editor of make xconfig, but you also have the text-mode menu-driven one in make menuconfig,
and if you happen to be really familiar with how this stuff works already, you can just dig in with nano, or vi, or your other
favorite text editor.

**NOTE: If you are not absolutely certain what you are doing, I recommend not taking anything OUT. Add what you need, but leave the
existing settings alone!**

I also recommend changing the CONFIG_LOCALVERSION setting, which is set to "-microsoft-standard", to a different version string. This
is useful later when you're trying to figure out which custom kernel you're actually running. It doesn't matter much what you change it
to, since it's an arbitrary identifier.

If you don't want to muck around with the config yourself, I have added to this gist my .config, which adds in squashfs, apparmor,
a few networking settings needed to get LXC containers up and running, and some other minor things. Feel free to use it.

## Step Five: Compile the Kernel

```
make
```

then wait. When you're done with the compilation, copy the compiled image to somewhere on the Windows filesystem (i.e., outside WSL).

```
cp arch/x86/boot/bzImage /mnt/c/Working
```

### Step Six: Exit WSL and shut it down

I.e., after you exit all your WSL sessions, run

```
wsl --shutdown
```

for good measure and wait for it to complete.

## Step Six: BACK UP THE OLD (MICROSOFT-PROVIDED) KERNEL

In Explorer, go to `C:\Windows\System32\lxss\tools` . There is a file in that folder called, obviously enough, `kernel` .

Rename that file to `kernel.old`. You will receive a UAC prompt, as you will also in the next step.

## Step Seven: Install your new kernel

Copy the `bzImage` file you copied out earlier (in Step Five) into its place, and rename it `kernel`.

## Step Eight: Try it!

Open up your WSL prompt, and see if it works. If it does, run `uname -a`. If you see the custom local version I suggested you set
back in Step Four in the output, then everything's working properly.

And that's how you change out your WSL 2 kernel. In conjunction with genie ( https://github.com/arkane-systems/genie ), that should
get most things working for you.

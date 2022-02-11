#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <fcntl.h>
#include <err.h>

#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>

#include <netinet/in.h>
#include <net/if.h>

#include <errno.h>

#include <sys/ioctl.h>
#include <net/bpf.h>

#include <net/ethernet.h>


int open_dev(void);
int check_dlt(int fd);
int set_options(int fd, char *iface);
int set_filter(int fd);
void read_packets(int fd);


    int
main(int argc, char *argv[])
{
    int fd = 0;
    char *iface = NULL;


    iface = strdup(argc < 2 ? "en1" : argv[1]);
    if (iface == NULL)
        err(EXIT_FAILURE, "strdup");

    fd = open_dev();
    if (fd < 0)
        err(EXIT_FAILURE, "open_dev");

    if (set_options(fd, iface) < 0)
        err(EXIT_FAILURE, "set_options");

    if (check_dlt(fd) < 0)
        err(EXIT_FAILURE, "check_dlt");

    if (set_filter(fd) < 0)
        err(EXIT_FAILURE, "set_filter");

    read_packets(fd);

    err(EXIT_FAILURE, "read_packets");
}


    int
open_dev()
{
    int fd = -1;
    char dev[32];
    int i = 0;


    /* Open the bpf device */
    for (i = 0; i < 255; i++) {
        (void)snprintf(dev, sizeof(dev), "/dev/bpf%u", i);

        (void)printf("Trying to open: %s\n", dev);

        fd = open(dev, O_RDWR);
        if (fd > -1)
            return fd;

        switch (errno) {
            case EBUSY:
                break;
            default:
                return -1;
        }
    }

    errno = ENOENT;
    return -1;
}

    int
check_dlt(int fd)
{
    u_int32_t dlt = 0;


    /* Ensure we are dumping the datalink we expect */
    if(ioctl(fd, BIOCGDLT, &dlt) < 0)
        return -1;

    (void)fprintf(stdout, "datalink type=%u\n", dlt);

    switch (dlt) {
        case DLT_EN10MB:
            return 0;
        default:
            (void)fprintf(stderr, "Unsupported datalink type:%u", dlt);
            errno = EINVAL;
            return -1;
    }
}

    int
set_options(int fd, char *iface)
{
    struct ifreq ifr;
    u_int32_t enable = 1;


    /* Associate the bpf device with an interface */
    (void)strlcpy(ifr.ifr_name, iface, sizeof(ifr.ifr_name)-1);

    if(ioctl(fd, BIOCSETIF, &ifr) < 0)
        return -1;

    /* Set header complete mode */
    if(ioctl(fd, BIOCSHDRCMPLT, &enable) < 0)
        return -1;

    /* Monitor packets sent from our interface */
    if(ioctl(fd, BIOCSSEESENT, &enable) < 0)
        return -1;

    /* Return immediately when a packet received */
    if(ioctl(fd, BIOCIMMEDIATE, &enable) < 0)
        return -1;

    return 0;
}

    int
set_filter(int fd)
{
    struct bpf_program fcode = {0};

    /* dump http packets only */
    int port = 80;
    struct bpf_insn insns[] = {
        BPF_STMT(BPF_LD+BPF_H+BPF_ABS, 12),
        BPF_JUMP(BPF_JMP+BPF_JEQ+BPF_K, ETHERTYPE_IP, 0, 10),
        BPF_STMT(BPF_LD+BPF_B+BPF_ABS, 23),
        BPF_JUMP(BPF_JMP+BPF_JEQ+BPF_K, IPPROTO_TCP, 0, 8),
        BPF_STMT(BPF_LD+BPF_H+BPF_ABS, 20),
        BPF_JUMP(BPF_JMP+BPF_JSET+BPF_K, 0x1fff, 6, 0),
        BPF_STMT(BPF_LDX+BPF_B+BPF_MSH, 14),
        BPF_STMT(BPF_LD+BPF_H+BPF_IND, 14),
        BPF_JUMP(BPF_JMP+BPF_JEQ+BPF_K, port, 2, 0),
        BPF_STMT(BPF_LD+BPF_H+BPF_IND, 16),
        BPF_JUMP(BPF_JMP+BPF_JEQ+BPF_K, port, 0, 1),
        BPF_STMT(BPF_RET+BPF_K, (u_int)-1),
        BPF_STMT(BPF_RET+BPF_K, 0),
    };


    /* Set the filter */
    fcode.bf_len = sizeof(insns) / sizeof(struct bpf_insn);
    fcode.bf_insns = &insns[0];

    if(ioctl(fd, BIOCSETF, &fcode) < 0)
        return -1;

    return 0;
}

    void
read_packets(int fd)
{
    char *buf = NULL;
    char *p = NULL;
    size_t blen = 0;
    ssize_t n = 0;
    struct bpf_hdr *bh = NULL;
    struct ether_header *eh = NULL;


    if(ioctl(fd, BIOCGBLEN, &blen) < 0)
        return;

    if ( (buf = malloc(blen)) == NULL)
        return;

    (void)printf("reading packets ...\n");

    for ( ; ; ) {
        (void)memset(buf, '\0', blen);

        n = read(fd, buf, blen);

        if (n <= 0)
            return;

        p = buf;
        while (p < buf + n) {
            bh = (struct bpf_hdr *)p;

            /* Start of ethernet frame */
            eh = (struct ether_header *)(p + bh->bh_hdrlen);

            (void)printf("%02x:%02x:%02x:%02x:%02x:%02x -> "
                    "%02x:%02x:%02x:%02x:%02x:%02x "
                    "[type=%u]\n",
                    eh->ether_shost[0], eh->ether_shost[1], eh->ether_shost[2],
                    eh->ether_shost[3], eh->ether_shost[4], eh->ether_shost[5],

                    eh->ether_dhost[0], eh->ether_dhost[1], eh->ether_dhost[2],
                    eh->ether_dhost[3], eh->ether_dhost[4], eh->ether_dhost[5],

                    eh->ether_type);

            p += BPF_WORDALIGN(bh->bh_hdrlen + bh->bh_caplen);
        }
    }
}
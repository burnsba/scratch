//   Copyright 2013 Benjamin Burns
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.


// playing around with generating sounds (and wav files)

#include <math.h>
#include <stdint.h>
#include <stdio.h>

#ifndef M_PI
    #define M_PI       3.14159265358979323846
#endif

typedef enum
{
    A_0,
    AS0,
    B_0,
    C_1,
    CS1,
    D_1,
    DS1,
    E_1,
    F_1,
    FS1,
    G_1,
    GS1,
    A_1,
    AS1,
    B_1,
    C_2,
    CS2,
    D_2,
    DS2,
    E_2,
    F_2,
    FS2,
    G_2,
    GS2,
    A_2,
    AS2,
    B_2,
    C_3,
    CS3,
    D_3,
    DS3,
    E_3,
    F_3,
    FS3,
    G_3,
    GS3,
    A_3,
    AS3,
    B_3,
    C_4,
    CS4,
    D_4,
    DS4,
    E_4,
    F_4,
    FS4,
    G_4,
    GS4,
    A_4,
    AS4,
    B_4,
    C_5,
    CS5,
    D_5,
    DS5,
    E_5,
    F_5,
    FS5,
    G_5,
    GS5,
    A_5,
    AS5,
    B_5,
    C_6,
    CS6,
    D_6,
    DS6,
    E_6,
    F_6,
    FS6,
    G_6,
    GS6,
    A_6,
    AS6,
    B_6,
    C_7,
    CS7,
    D_7,
    DS7,
    E_7,
    F_7,
    FS7,
    G_7,
    GS7,
    A_7,
    AS7,
    B_7,
    C_8,
    
} PIANO_NOTE;

double piano_note_to_frequency(PIANO_NOTE note)
{
    switch (note)
    {
        case (A_0): return 27.5;
        case (AS0): return 29.1352;
        case (B_0): return 30.8677;
        case (C_1): return 32.7032;
        case (CS1): return 34.6478;
        case (D_1): return 36.7081;
        case (DS1): return 38.8909;
        case (E_1): return 41.2034;
        case (F_1): return 43.6535;
        case (FS1): return 46.2493;
        case (G_1): return 48.9994;
        case (GS1): return 51.9131;
        case (A_1): return 55;
        case (AS1): return 58.2705;
        case (B_1): return 61.7354;
        case (C_2): return 65.4064;
        case (CS2): return 69.2957;
        case (D_2): return 73.4162;
        case (DS2): return 77.7817;
        case (E_2): return 82.4069;
        case (F_2): return 87.3071;
        case (FS2): return 92.4986;
        case (G_2): return 97.9989;
        case (GS2): return 103.826;
        case (A_2): return 110;
        case (AS2): return 116.541;
        case (B_2): return 123.471;
        case (C_3): return 130.813;
        case (CS3): return 138.591;
        case (D_3): return 146.832;
        case (DS3): return 155.563;
        case (E_3): return 164.814;
        case (F_3): return 174.614;
        case (FS3): return 184.997;
        case (G_3): return 195.998;
        case (GS3): return 207.652;
        case (A_3): return 220;
        case (AS3): return 233.082;
        case (B_3): return 246.942;
        case (C_4): return 261.626;
        case (CS4): return 277.183;
        case (D_4): return 293.665;
        case (DS4): return 311.127;
        case (E_4): return 329.628;
        case (F_4): return 349.228;
        case (FS4): return 369.994;
        case (G_4): return 391.995;
        case (GS4): return 415.305;
        case (A_4): return 440;
        case (AS4): return 466.164;
        case (B_4): return 493.883;
        case (C_5): return 523.251;
        case (CS5): return 554.365;
        case (D_5): return 587.33;
        case (DS5): return 622.254;
        case (E_5): return 659.255;
        case (F_5): return 698.456;
        case (FS5): return 739.989;
        case (G_5): return 783.991;
        case (GS5): return 830.609;
        case (A_5): return 880;
        case (AS5): return 932.328;
        case (B_5): return 987.767;
        case (C_6): return 1046.5;
        case (CS6): return 1108.73;
        case (D_6): return 1174.66;
        case (DS6): return 1244.51;
        case (E_6): return 1318.51;
        case (F_6): return 1396.91;
        case (FS6): return 1479.98;
        case (G_6): return 1567.98;
        case (GS6): return 1661.22;
        case (A_6): return 1760;
        case (AS6): return 1864.66;
        case (B_6): return 1975.53;
        case (C_7): return 2093;
        case (CS7): return 2217.46;
        case (D_7): return 2349.32;
        case (DS7): return 2489.02;
        case (E_7): return 2637.02;
        case (F_7): return 2793.83;
        case (FS7): return 2959.96;
        case (G_7): return 3135.96;
        case (GS7): return 3322.44;
        case (A_7): return 3520;
        case (AS7): return 3729.31;
        case (B_7): return 3951.07;
        case (C_8): return 4186.01;
        
        default:
            return 0;
    }
}

void write_sound(FILE* wav_file, unsigned int wav_sample_rate, unsigned int wav_volume, unsigned int number_samples, double frequency)
{
    int t;
    
    if (!wav_file)
        return;
        
    for (t=0; t<number_samples ;t++)
    {
        uint8_t c;
        c = (uint8_t)(sin(t*2*M_PI/wav_sample_rate*frequency)+1)*wav_volume;
        fputc(c, wav_file);
    }
}

void write_little_endian(unsigned int word, int num_bytes, FILE *wav_file)
{
    unsigned buf;
    while(num_bytes>0)
    {   buf = word & 0xff;
        fwrite(&buf, 1,1, wav_file);
        num_bytes--;
        word >>= 8;
    }
}

int main()
{
    const double V=127;
    
    FILE * wav_file;

    unsigned int sample_rate = 8000;
    unsigned int num_channels = 1;
    unsigned int bytes_per_sample;
    unsigned int byte_rate;
    unsigned int num_samples = 76000;
    
    wav_file = fopen("out.wav", "w");
 
    num_channels = 1;   /* monoaural */
    bytes_per_sample = 2;
    byte_rate = sample_rate*num_channels*bytes_per_sample;
    
    // https://ccrma.stanford.edu/courses/422/projects/WaveFormat/
 
    /* write RIFF header */
    fwrite("RIFF", 1, 4, wav_file);
    write_little_endian(36 + bytes_per_sample* num_samples*num_channels, 4, wav_file);
    fwrite("WAVE", 1, 4, wav_file);
 
    /* write fmt  subchunk */
    fwrite("fmt ", 1, 4, wav_file);
    write_little_endian(16, 4, wav_file);   /* SubChunk1Size is 16 */
    write_little_endian(1, 2, wav_file);    /* PCM is format 1 */
    write_little_endian(num_channels, 2, wav_file);
    write_little_endian(sample_rate, 4, wav_file);
    write_little_endian(byte_rate, 4, wav_file); 
    write_little_endian(num_channels*bytes_per_sample, 2, wav_file);  /* block align */
    write_little_endian(8*bytes_per_sample, 2, wav_file);  /* bits/sample */
 
    /* write data subchunk */
    fwrite("data", 1, 4, wav_file);
    write_little_endian(bytes_per_sample* num_samples*num_channels, 4, wav_file);
    
    ////////////////////////////////////////////////////////////////
    
    write_sound(wav_file, sample_rate, V, 8000, piano_note_to_frequency(C_4));
    write_sound(wav_file, sample_rate, V, 8000, piano_note_to_frequency(E_4));
    write_sound(wav_file, sample_rate, V, 4000, 0);
    write_sound(wav_file, sample_rate, V, 8000, piano_note_to_frequency(G_4));
    write_sound(wav_file, sample_rate, V, 4000, 0);
    write_sound(wav_file, sample_rate, V, 4000, piano_note_to_frequency(G_4));
    write_sound(wav_file, sample_rate, V, 4000, 0);
    write_sound(wav_file, sample_rate, V, 4000, piano_note_to_frequency(G_4));
    write_sound(wav_file, sample_rate, V, 4000, 0);
    write_sound(wav_file, sample_rate, V, 4000, piano_note_to_frequency(G_4));
    write_sound(wav_file, sample_rate, V, 24000, 0);

    fclose(wav_file);

    printf("done.\n");
    
    return 0;
}
